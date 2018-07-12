using System.Collections.Generic;
using System.Text;
using Orange.Library.Managers;
using Orange.Library.Messages;
using Orange.Library.Parsers;
using Orange.Library.Patterns;
using Orange.Library.Replacements;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Standard.Types.Booleans.Assertions;

namespace Orange.Library.Values
{
   public class Pattern : Value, IMessageHandler
   {
      const string DIV_MAJOR = "////////////////////";
      const string DIV_MINOR = "--------------------";
      const string LOCATION = "Pattern";
      const string REGEX_LIMIT = "^ '$' /(/d+) 'x'";
      const string REGEX_NTH = "^ '$' /(/d+) ('th' | 'st' | 'rd' | 'nd')";

      public static explicit operator Pattern(string source)
      {
         if (!source.IsMatch("^ /s+"))
            source = " " + source;
         var parser = new PatternParser();
         Assert(parser.Scan(source, 0), $"Could not parse pattern {source}");

         return (Pattern)parser.Result.Value;
      }

      Element head;
      int startIndex;
      int stopIndex;
      bool multiScan;
      int limit;
      int nth;
      Matcher matcher;

      public Pattern(Element head)
      {
         this.head = head;
         multiScan = false;
         limit = -1;
         nth = -1;
         matcher = new Matcher();
      }

      public Pattern()
         : this(null) { }

      public bool Direct { get; set; }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "do", v => v.Do(false));
         manager.RegisterMessage(this, "all", v => v.Do(true));
         manager.RegisterMessage(this, "times", v => ((Pattern)v).Times());
         manager.RegisterMessage(this, "trace", v => ((Pattern)v).Trace());
         manager.RegisterMessage(this, "isMatch", v => v.IsTrue);
         manager.RegisterMessage(this, "rp", v => ((Pattern)v).SetRecordPattern());
         manager.RegisterMessage(this, "fp", v => ((Pattern)v).SetFieldPattern());
         manager.RegisterMessage(this, "split", v => ((Pattern)v).Split());
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((Pattern)v).Apply());
         manager.RegisterMessageCall("applyWhile");
         manager.RegisterMessage(this, "applyWhile", v => ((Pattern)v).ApplyWhile());
         manager.RegisterMessage(this, "count", v => ((Pattern)v).Count());
         manager.RegisterMessageCall("applyNot");
         manager.RegisterMessage(this, "applyNot", v => ((Pattern)v).ApplyNot());
         manager.RegisterProperty(this, "limit", v => ((Pattern)v).GetLimit(), v => ((Pattern)v).SetLimit());
         manager.RegisterMessage(this, "for", v => ((Pattern)v).For());
         manager.RegisterMessage(this, "while", v => ((Pattern)v).While());
         manager.RegisterMessage(this, "matches", v => ((Pattern)v).Matches());
         manager.RegisterMessage(this, "fail", v => ((Pattern)v).Fail());
         manager.RegisterMessage(this, "replace", v => ((Pattern)v).Replace());
         manager.RegisterMessage(this, "replaceAll", v => ((Pattern)v).ReplaceAll());
      }

      public Value Limit() => new ValueAttributeVariable("limit", this);

      public Value GetLimit() => limit;

      public Value SetLimit()
      {
         limit = (int)Arguments[0].Number;
         return this;
      }

      public Value ApplyNot()
      {
         var result = (PatternResult)Apply();
         result.Success = !result.Success;
         return result;
      }

      public Value Split()
      {
         var value = Arguments[0];
         return new Array(Split(value.Text));
      }

      public Value SetRecordPattern()
      {
         var executable = Arguments.Executable;
         if (executable.CanExecute)
         {
            var oldPattern = State.RecordPattern;
            State.RecordPattern = this;
            var value = executable.Evaluate();
            State.RecordPattern = oldPattern;
            return value;
         }

         State.RecordPattern = this;
         return this;
      }

      public Value SetFieldPattern()
      {
         var executable = Arguments.Executable;
         if (executable.CanExecute)
         {
            var oldPattern = State.FieldPattern;
            State.FieldPattern = this;
            var value = executable.Evaluate();
            State.FieldPattern = oldPattern;
            return value;
         }

         State.FieldPattern = this;
         return this;
      }

      public Value Trace()
      {
         State.PushPatternManager();
         State.Trace = true;
         var value = Arguments[0];
         var variableName = Arguments.VariableName(0, VAR_PATTERN_INPUT);
         var input = value.Text;
         var oneSuccess = Scan(input);
         if (oneSuccess && variableName.IsNotEmpty())
         {
            input = State.Input;
            State.PopPatternManager();
            return input;
         }

         State.PopPatternManager();
         return false;
      }

      public Value Times()
      {
         var count = Arguments[1];
         return Do((int)count.Number);
      }

      public IReplacement Replacement { get; set; }

      public Element Head => head;

      public override string ToString() => Source;

      public Value Apply()
      {
         var argument = Arguments.ApplyValue;
         var variable = Arguments.ApplyVariable;
         State.PushPatternManager();
         var isVariable = variable != null;
         var isReadOnly = isVariable && Regions.IsReadOnly(variable.Name) || !isVariable;
         string input;
         if (argument is PatternResult patternResult)
            if (patternResult.Success)
            {
               input = patternResult.Text;
               variable = patternResult.Variable;
               isVariable = variable != null;
            }
            else
               return PatternResult.Falure();
         else
            input = argument.Text;

         var matched = Scan(input);
         var text = State.Input.Copy();
         if (isReadOnly)
         {
            State.PopPatternManager();
            return matched ? (Value)new Some(text.Skip(startIndex).Take(stopIndex - startIndex)) : new None();
         }

         var result = matched ? new PatternResult
         {
            Input = input,
            Text = text,
            Value = argument,
            Success = true,
            StartIndex = startIndex,
            StopIndex = stopIndex,
            Variable = variable
         } : PatternResult.Falure();
         if (isVariable && result.IsTrue)
            variable.Value = text;
         if (State.Result != null)
         {
            var managerResult = State.Result;
            State.Result = null;
            State.PopPatternManager();
            return managerResult;
         }

         State.PopPatternManager();
         return result;
      }

      bool withinLimit(int index) => limit == -1 || index < limit;

      bool withinNth(int index) => nth == -1 || index == nth;

      public Value ApplyWhile()
      {
         var argument = Arguments.ApplyValue;
         var variable = Arguments.ApplyVariable;
         State.PushPatternManager();
         State.Multi = true;
         var isVariable = variable != null;
         var isReadOnly = isVariable && Regions.IsReadOnly(variable.Name);
         var array = new Array();
         string input;
         switch (argument)
         {
            case PatternResult patternResult when patternResult.Success:
               input = patternResult.Text;
               variable = patternResult.Variable;
               isVariable = variable != null;
               break;
            case PatternResult _:
               return PatternResult.Falure();
            default:
               input = argument.Text;
               break;
         }

         var oneSuccess = false;

         for (var i = 0; i < MAX_LOOP && withinLimit(i); i++)
         {
            if (Scan(input))
            {
               State.Alternates.Clear();
               if (withinNth(i))
                  oneSuccess = true;
            }
            else
               break;

            input = State.Input;
            if (isReadOnly)
               array.Add(input.Skip(startIndex).Take(stopIndex - startIndex));
         }

         if (isReadOnly)
         {
            State.PopPatternManager();
            return oneSuccess ? array : new Array();
         }

         var result = oneSuccess ? new PatternResult
         {
            Input = input,
            Text = State.Input.Copy(),
            Value = argument,
            Success = true,
            StartIndex = startIndex,
            StopIndex = stopIndex,
            Variable = variable
         } : PatternResult.Falure();
         if (isVariable && result.IsTrue)
            variable.Value = State.Input.Copy();
         if (State.Result != null)
         {
            var managerResult = State.Result;
            State.Result = null;
            State.PopPatternManager();
            return managerResult;
         }

         State.PopPatternManager();
         return result;
      }

      public Element LastElement { get; set; }

      public int Index => startIndex;

      public int Length => stopIndex - startIndex;

      void trace(Element current, string input, bool success)
      {
         if (!State.Trace)
            return;

         var state = State;

         var length = stopIndex - startIndex;

         var builder = new StringBuilder("¬".Repeat(state.PatternDepth - 1));
         builder.Append(current);
         builder.Append($":{(success ? "S" : "F")} ");
         builder.Append($"'{input.Skip(startIndex).Take(length)}' ");
         builder.Append($"({startIndex}:{length})");
         state.Print(builder.ToString());
      }

      static void traceMinor() => State.Print(DIV_MINOR);

      static void traceMajor() => State.Print(DIV_MAJOR);

      bool result(bool success)
      {
         State.PatternDepth--;
         if (success)
         {
            if (startIndex > -1 && stopIndex > -1 && Replacement != null)
            {
               if (multiScan)
                  Replacement = Replacement.Clone();
               State.SaveWorkingInput();
               var slicer = new Slicer(State.WorkingInput);
               var length = stopIndex - startIndex;
               var text = slicer[startIndex, length];
               State.WorkingInput = text;
               Regions.SetPatternPositionData(startIndex, length);
               if (Replacement.Immediate)
               {
                  var replacementText = Replacement.Text;
                  if (replacementText != null)
                  {
                     slicer[startIndex, length] = replacementText;
                     State.RestoreWorkingInput();
                     State.WorkingInput = slicer.ToString();
                  }
                  else
                     State.RestoreWorkingInput();
               }
               else
               {
                  State.Replacements.Add(startIndex, length, Replacement);
                  State.RestoreWorkingInput();
               }
            }

            if (!SubPattern)
               State.Replacements.Replace();
         }

         return success;
      }

      public bool SubPattern { get; set; }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            if (Arguments == null)
               return "";

            State.PushPatternManager();
            var value = Arguments[0];
            var input = value.Text;
            var result = Scan(input) ? input.Skip(startIndex).Take(Length) : "";
            State.PopPatternManager();
            return result;
         }
         set { }
      }

      public override double Number
      {
         get => Text.ToDouble();
         set { }
      }

      public override ValueType Type => ValueType.Pattern;

      public override bool IsTrue
      {
         get
         {
            if (Arguments == null)
               return false;

            State.PushPatternManager();
            var value = Arguments[0];
            var input = value.Text;
            var success = Scan(input);
            State.PopPatternManager();
            return success;
         }
      }

      public override Value Clone() => new Pattern(head.Clone());

      public bool Scan(string input)
      {
         if (input.IsEmpty())
            return false;

         State.PatternDepth++;
         if (!SubPattern)
         {
            State.IgnoreCase = !options[OptionType.Case];
            State.Anchored = options[OptionType.Anchor];
            State.Input = input;
         }

         startIndex = -1;
         stopIndex = -1;

         var current = head;
         var firstIndex = State.Position;
         var textLength = input.Length;
         var warningLength = textLength + 1000;
         var stuckLimit = textLength + 1000;
         var stuckAt = 0;
         var stuckIndex = -1;

         while (current != null)
         {
            Assert(textLength < warningLength, "Runaway text");
            if (startIndex == stuckIndex)
            {
               if (++stuckAt > stuckLimit)
                  throw "Past stuck limit".Throws();
            }
            else
            {
               stuckAt = 0;
               stuckIndex = startIndex;
            }

            Reject(textLength > MAX_PATTERN_INPUT_LENGTH && multiScan && State.Position > 0, "Text out of control");
            current.Initialize();

            if (current.Alternate != null)
            {
               var alternateState = new AlternateData
               {
                  Alternate = current.Alternate,
                  Position = State.Position,
                  Next = current.Next,
                  OwnerNext = OwnerNext,
                  ElementID = ID,
                  Exit = SubPattern
               };
               alternateState.Alternate.Replacement = current.Replacement ?? OwnerReplacement;
               State.Alternates.Push(alternateState);
            }

            if ((State.FirstScan ? current.EvaluateFirst(input) : current.Evaluate(input)) && current.Valid)
            {
               State.FirstScan = false;
               var replacement = current.Replacement;
               if (replacement != null)
               {
                  var text = input.Skip(current.Index).Take(current.Length);

                  if (multiScan)
                  {
                     replacement = replacement.Clone();
                     current.Replacement = replacement;
                  }

                  addToReplacements(replacement, text, current);
               }

               if (startIndex == -1)
                  startIndex = current.Index;
               if (!current.PositionAlreadyUpdated)
                  State.Position = current.Index + current.Length;
               stopIndex = State.Position;
               Assert(stopIndex >= startIndex, "Stop index can't be less than start index");
               if (current.Next == null)
                  return result(true);

               current = current.Next;
            }
            else
            {
               State.FirstScan = false;
               if (State.Aborted || SubPattern && !(State.Alternates.Count > 0 &&
                  State.Alternates.Peek().ElementID == ID))
               {
                  State.Aborted = false;
                  return result(false);
               }

               if (current.Aborted)
               {
                  State.Aborted = true;
                  return result(false);
               }

               if (State.Alternates.Count > 0)
               {
                  var peek = State.Alternates.Peek();
                  var noExit = peek.ElementID == ID;
                  if (SubPattern && current.Next == null && !noExit)
                     return result(false);

                  var oldNext = current.Next;
                  var alternate = State.Alternates.Pop();
                  if (noExit)
                     alternate.OwnerNext = null;
                  current = alternate.Alternate;
                  current.Next = alternate.Next ?? alternate.OwnerNext;
                  current.ID = alternate.ElementID;
                  State.Position = alternate.Position;
                  if (startIndex > State.Position)
                     startIndex = State.Position;
                  if (SubPattern && oldNext == null && !noExit)
                     return result(false);
               }
               else
               {
                  State.Replacements.Clear();
                  if (State.Anchored || State.Position >= textLength)
                     return result(false);

                  State.Position = ++firstIndex;
                  if (State.Position >= textLength)
                  {
                     State.Position--;
                     return result(false);
                  }

                  current = head;
                  startIndex = -1;
                  stopIndex = -1;
                  State.Alternates.Clear();
               }
            }
         }

         return result(false);
      }

      static void addToReplacements(IReplacement replacement, string text, Element current)
      {
         if (replacement.Immediate)
         {
            State.SaveWorkingInput();
            State.WorkingInput = text;
            var arguments = new Arguments();
            arguments.AddArgument(text);
            arguments.AddArgument(current.Index);
            arguments.AddArgument(current.Length);
            replacement.Arguments = arguments;
            replacement.Evaluate();
            State.RestoreWorkingInput();
         }
         else
            State.Replacements.Add(current.Index, current.Length, replacement);
      }

      public IReplacement OwnerReplacement { get; set; }

      public Element OwnerNext { get; set; }

      public override Value Do(bool repeat)
      {
         State.PushPatternManager();
         multiScan = repeat;
         var oneSuccess = false;
         var value = Arguments[0];
         var variableName = Arguments.VariableName(0, VAR_PATTERN_INPUT);
         var input = value.Text;
         if (repeat)
         {
            State.Multi = true;
            for (var i = 0; i < MAX_LOOP; i++)
            {
               if (Scan(input))
                  oneSuccess = true;
               else
                  break;

               input = State.Input;
            }
         }
         else
            oneSuccess = Scan(input);

         if (oneSuccess && variableName.IsNotEmpty())
         {
            Regions[variableName] = State.Input;
            State.PopPatternManager();
            return null;
         }

         State.PopPatternManager();
         return input;
      }

      public override Value Do(int count)
      {
         State.PushPatternManager();
         multiScan = true;
         var value = State.Stack.Pop(false, LOCATION);
         var isVariable = value.IsVariable;
         var input = value.Text;
         State.Multi = true;
         for (var i = 0; i < count; i++)
            if (!Scan(input))
               break;

         if (isVariable)
         {
            var variable = (Variable)value;
            variable.Value = input;
            State.PopPatternManager();
            return null;
         }

         State.PopPatternManager();
         return input;
      }

      public string[] Split(string input)
      {
         State.PushPatternManager();
         var array = new List<string>();
         var lastPosition = -1;
         while (State.Position != lastPosition)
         {
            State.FirstScan = true;
            lastPosition = State.Position;
            if (!Scan(input))
               break;

            State.Alternates.Clear();
            var text = input.Skip(lastPosition).Take(State.Position - lastPosition - Length);
            array.Add(text);
         }

         array.Add(input.Skip(lastPosition).Take(State.Position - lastPosition - Length + 1));
         State.PopPatternManager();
         return array.ToArray();
      }

      public bool IsMatch(string input)
      {
         State.PushPatternManager();
         var result = Scan(input);
         State.PopPatternManager();
         return result;
      }

      public bool MatchAndBind(string input)
      {
         State.PushPatternManager();
         var result = Scan(input);
         if (result)
            State.AssignPatternBindings();
         State.PopPatternManager();
         return result;
      }

      public int Find(string input, int at, out int length)
      {
         State.PushPatternManager();
         if (Scan(input.Skip(at)))
         {
            length = Length;
            State.PopPatternManager();
            return Index + at;
         }

         length = 0;
         State.PopPatternManager();
         return -1;
      }

      public int Count(string input)
      {
         var count = 0;
         State.PushPatternManager();
         State.Multi = true;
         while (Scan(input))
            count++;

         State.PopPatternManager();
         return count;
      }

      public Value Count() => Count(Arguments[0].Text);

      public Value For()
      {
         var input = Arguments[0].Text;
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return new Nil();

            assistant.ReplacementParameters();

            State.PushPatternManager();
            State.Multi = true;
            for (var i = 0; i < MAX_LOOP && withinLimit(i); i++)
            {
               if (Scan(input))
               {
                  State.Alternates.Clear();
                  assistant.SetReplacement(State.Input.Skip(Index).Take(Length), Index, Length, i);
                  var blockResult = block.Evaluate();
                  var signal = ParameterAssistant.Signal();
                  if (signal == Breaking)
                     break;

                  switch (signal)
                  {
                     case ReturningNull:
                     {
                        State.PopPatternManager();
                        return null;
                     }
                     case Continuing:
                        continue;
                  }

                  if (blockResult != null && !blockResult.IsNil)
                  {
                     Slicer slicer = State.Input;
                     var oldLength = slicer.Length;
                     slicer[Index, Length] = blockResult.Text;
                     var offset = slicer.Length - oldLength;
                     State.Input = slicer.ToString();
                     State.Position += offset;
                  }
               }
               else
                  break;

               input = State.Input;
            }

            State.PopPatternManager();
            return null;
         }
      }

      public Value While()
      {
         var value = Arguments[0];
         var variableName = value.Text;
         Reject(variableName.IsEmpty(), LOCATION, "Variable name not set for .while");
         var variable = new Variable(variableName);

         var oneSuccess = false;
         string originalInput = null;
         for (var i = 0; i < MAX_LOOP && withinLimit(i); i++)
         {
            var input = variable.Value.Text;
            if (originalInput == null)
               originalInput = input.Copy();
            State.PushPatternManager();
            State.Multi = true;
            if (Scan(input))
            {
               if (withinNth(i))
               {
                  oneSuccess = true;
                  variable.Value = State.Input.Copy();
                  State.PopPatternManager();
               }
            }
            else
            {
               State.PopPatternManager();
               break;
            }
         }

         var result = oneSuccess ? new PatternResult
         {
            Input = originalInput,
            Text = variable.Value.Text,
            Value = variable.Value,
            Success = true,
            StartIndex = startIndex,
            StopIndex = stopIndex,
            Variable = variable
         } : PatternResult.Falure();
         return result;
      }

      public string Source { get; set; }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         if (matcher.IsMatch(messageName, REGEX_LIMIT))
         {
            limit = matcher[0, 1].ToInt();
            handled = true;
            return this;
         }

         if (matcher.IsMatch(messageName, REGEX_NTH))
         {
            nth = matcher[0, 1].ToInt() - 1;
            handled = true;
            return this;
         }

         handled = false;
         return null;
      }

      public bool RespondsTo(string messageName) =>
         matcher.IsMatch(messageName, REGEX_LIMIT) || matcher.IsMatch(messageName, REGEX_NTH);

      public Value Matches()
      {
         var input = Arguments[0].Text;
         State.PushPatternManager();
         var array = new Array();
         while (Scan(input))
         {
            var text = input.Skip(State.Position - Length).Take(Length);
            array.Add(text);
         }

         State.PopPatternManager();
         return array;
      }

      public void SetOverridenReplacement(IReplacement value)
      {
         Replacement = value;
         if (Replacement?.FixedID.IsNone == true)
            Replacement.FixedID = CompilerState.ObjectID().Some();
      }

      public Value Fail()
      {
         var input = Arguments[0].Text;
         for (var i = 0; i < input.Length && withinLimit(i); i++)
         {
            var toScan = input.Skip(i);
            State.PushPatternManager();
            State.Multi = true;
            Scan(toScan);
            State.PopPatternManager();
         }

         return new PatternResult
         {
            Input = input,
            Text = input.Skip(startIndex).Take(stopIndex - startIndex),
            Value = input,
            Success = true,
            StartIndex = startIndex,
            StopIndex = stopIndex,
            Variable = null
         };
      }

      public Value Replace() => Replace(Arguments[0].Text);

      public Value Replace(string input)
      {
         State.PushPatternManager();
         if (Scan(input))
         {
            var result = State.Input;
            State.PopPatternManager();
            return result;
         }

         State.PopPatternManager();
         return input;
      }

      public Value ReplaceAll() => ReplaceAll(Arguments[0].Text);

      public Value ReplaceAll(string input)
      {
         var original = input.Copy();
         State.PushPatternManager();
         var oneSuccess = false;
         for (var i = 0; i < MAX_LOOP && withinLimit(i); i++)
         {
            if (Scan(input))
            {
               State.Alternates.Clear();
               if (withinNth(i))
                  oneSuccess = true;
            }
            else
               break;

            input = State.Input;
         }

         input = State.Input;
         State.PopPatternManager();
         return oneSuccess ? input : original;
      }
   }
}