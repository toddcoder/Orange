using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Standard.Types.Booleans.Assertions;

namespace Orange.Library.Debugging
{
   public class Debugger
   {
      public enum DebugModeType
      {
         Run,
         Step,
         StepInto
      }

      public static bool IsDebugging => DebuggingState != null;

      public static bool CanAssert => IsDebugging && DebuggingState.CurrentVerb != null;

      public static Debugger DebuggingState { get; set; }

      public int CurrentLine { get; set; }

      StringBuilder currentLine;
      Position position;
      int linePostion;
      List<Line> lines;
      List<bool> breakpoints;

      public delegate void StepHandler(Position position);

      public event StepHandler Step;

      public Debugger()
      {
         currentLine = new StringBuilder();
         lines = new List<Line>();
         breakpoints = new List<bool>();
         position = new Position();
         linePostion = 0;
         DebugMode = DebugModeType.Run;
      }

      public Verb CurrentVerb { get; set; }

      public DebugModeType DebugMode { get; set; }

      public void AddSource(string source)
      {
         var sourceLength = source.Length;
         linePostion = currentLine.Length;
         currentLine.Append(source);
         position.Length += sourceLength;
      }

      public void EndSource(string source)
      {
         AddSource(source);
         Final();
      }

      public void Final()
      {
         lines.Add(new Line
         {
            Source = currentLine.ToString(),
            Position = position
         });
         breakpoints.Add(false);
         currentLine.Clear();
         position.Length = 0;
      }

      public int LineNumber => lines.Count;

      public int LinePosition => linePostion;

      string formattedMessage(string format, object[] args) => displaySource() + ": " + string.Format(format, args);

      string displaySource()
      {
         if (CurrentVerb == null)
            return "[no source available]";

         var line = lines[CurrentVerb.LineNumber];
         return line.Source.Skip(CurrentVerb.LinePosition);
      }

      string formattedMessageOnly(string format) => $"{displaySource()}: {format}";

/*      public void Assert(bool test, string message) => test.Assert(message);

      public void Reject(bool test, string message) => test.Reject(message);

      public void RejectNull(object obj, string message) => obj.AssertIsNotNull(message);*/

      public void SetBreakpoint(int lineNumber, bool value)
      {
         checkBreakpointRange(lineNumber);
         breakpoints[lineNumber] = value;
      }

      void checkBreakpointRange(int lineNumber) => Assert(lineNumber > -1 && lineNumber < breakpoints.Count, "Breakpoint out of range");

      void checkLineRange(int lineNumber) => Assert(lineNumber > -1 && lineNumber < lines.Count, "Line out of range");

      public void ToggleBreakpoint(int lineNumber)
      {
         checkBreakpointRange(lineNumber);
         breakpoints[lineNumber] ^= true;
      }

      public bool IsBreakpointSet(int lineNumber)
      {
         checkBreakpointRange(lineNumber);
         return breakpoints[lineNumber];
      }

      public void ClearAllBreakpoints()
      {
         for (var i = 0; i < breakpoints.Count; i++)
            breakpoints[i] = false;
      }

      public Line Line(int lineNumber)
      {
         checkLineRange(lineNumber);
         return lines[lineNumber];
      }

      public void Begin() => linePostion = 0;

      public Value Evaluate(Block block)
      {
         if (block.AutoRegister)
            Runtime.State.RegisterBlock(block);
         var stack = Runtime.State.Stack;

         foreach (var verb in block.Verbs.TakeWhile(verb => !Runtime.State.ExitSignal && !Runtime.State.SkipSignal))
         {
            linePostion = verb.LineNumber;
            CurrentVerb = verb;
            var line = lines[linePostion];

            Value value = "";
            if (verb is IEnd end)
            {
               if (end.EvaluateFirst)
                  value = verb.Evaluate();
               if (end.IsEnd)
               {
                  if (Runtime.State.DefaultParameterNames.PopAtEnd)
                     Runtime.State.ClearDefaultParameterNames();
                  stack.Clear();
               }
               else
                  stack.Push(value);
               obeyMode(line);
               continue;
            }

            value = verb.Evaluate();
            if (value == null)
               continue;

            var valueAsBlock = value.AsBlock;
            if (valueAsBlock != null && block.Expression)
               value = valueAsBlock.Evaluate();
            if (value != null)
               stack.Push(value);

            if (!Runtime.State.ReturnSignal)
               continue;

            var returnValue = Runtime.State.ReturnValue.Resolve();
            if (returnValue is IStringify stringify1)
               returnValue = stringify1.String;
            obeyMode(line);
            if (block.AutoRegister)
               Runtime.State.UnregisterBlock();
            return returnValue ?? "";
         }

         var result = stack.IsEmpty ? null : stack.Pop(true).ArgumentValue();
         if (result is IStringify stringify2)
            result = stringify2.String;
         if (block.AutoRegister)
            Runtime.State.UnregisterBlock();
         return result ?? "";
      }

      void obeyMode(Line line)
      {
         switch (DebugMode)
         {
            case DebugModeType.Step:
            case DebugModeType.StepInto:
               Step?.Invoke(line.Position);
               break;
            case DebugModeType.Run:
               if (breakpoints[linePostion])
                  Step?.Invoke(line.Position);
               break;
         }
      }
   }
}