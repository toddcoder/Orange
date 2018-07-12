using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orange.Library.Managers;
using Orange.Library.Parsers;
using Orange.Library.Patterns;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Computer;
using Standard.Types.Booleans;
using Standard.Types.Collections;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.Objects;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static System.String;
using static Orange.Library.Debugging.Debugger;
using static Orange.Library.Managers.RegionManager;
using static Standard.Types.Maybe.MaybeFunctions;
using Array = Orange.Library.Values.Array;
using Object = Orange.Library.Values.Object;
using String = Orange.Library.Values.String;

namespace Orange.Library
{
   public class Runtime
   {
      public const int MAX_VAR_DEPTH = 256;
      public const int MAX_BLOCK_DEPTH = MAX_VAR_DEPTH;
      public const int MAX_LOOP = 2048;
      public const int MAX_PATTERN_INPUT_LENGTH = 4048;
      public const int MAX_ARRAY = 2048;
      public const int MAX_RECURSION = 2048;
      public const int MAX_TAILCALL = 8192;

      public const string VAR_PATTERN_INPUT = "$_";
      public const string VAR_AT = "$at";
      public const string VAR_LENGTH = "$len";
      public const string VAR_ANONYMOUS = "$anonymous";
      public const string VAR_KEY = "$k";
      public const string VAR_VALUE = "_";
      public const string VAR_INDEX = "$i";
      public const string VAR_X = "$xVar";
      public const string VAR_Y = "$yVar";
      public const string VAR_PADDER = "$padder";
      public const string VAR_WATCH = "$watch";
      public const string VAR_ARRAY = "$array";
      public const string VAR_AUTOASSIGN = "$auto-assign";
      public const string VAR_XMETHOD = "$xm_{0}";
      public const string VAR_MANGLE = "__$";
      public const string VAR_ACCUM = VAR_MANGLE + "accum";

      public const string VAL_REGION = "__$region";
      public const string VAL_BLOCK_RANGE_SET = "__set";
      public const string VAL_MATCH_VALUE = "__match";

      public const string REGEX_VARIABLE = "['A-Za-z_$'] (['A-Za-z_0-9']*)";
      //public const string REGEX_VARIABLE = "['A-Za-z_'] (['A-Za-z_0-9']*)";
      public const string REGEX_VARIABLE1 = "['A-Za-z_$'";
      public const string REGEX_BEGIN_PATTERN = "'{'"; //"'“'";
      public const string REGEX_END_PATTERN = "'}'"; //"'”'";
      public const string REGEX_PARAMETER = "'|' /('*')? (> ('?'" + REGEX_VARIABLE + ") | '|')";
      public const string REGEX_SEND_MESSAGE = "/(/s*) /('::'? | '&') /(" + REGEX_VARIABLE + ") /('(' | '{:(')?";
      public const string REGEX_ATTRIBUTE_MESSAGE = "^ '__$' /('set' | 'get' | 'bef' | 'aft' | 'req' | 'ens' | 'inv') '_' -(> 'item' /b) /(.+) $";
      public const string REGEX_END = "/((^ /r/n) | (^ /r) | (^ /n))";
      public const string REGEX_END1 = "/((/r/n) | (/r) | (/n))";
      public const string REGEX_EXP_END = "[',:)]}']";
      public const string REGEX_ASSIGN = "/(' '*) /('**' | '*' | '+' | '-' | '//' | '~' | '%' | '??')? /('=' /s*)";

      public const string STRING_SPACES = " \r\n\t";
      public const string STRING_DIGITS = "0123456789";
      public const string STRING_WORDS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
      public const string STRING_LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
      public const string STRING_PUNCT = @"!""#$%&'()*+,\./:;<=>?@[\\\]^_`{|}~-";
      public const string STRING_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      public const string STRING_LOWER = "abcdefghijklmnopqrstuvwxyz";
      public const string STRING_VOWELS = "AEIOUaeiou";
      public const string STRING_UVOWELS = "AEIOU";
      public const string STRING_LVOWELS = "aeiou";
      public const string STRING_UCONSONANTS = "BCDFGHJKLMNPQRSTVWXYZ";
      public const string STRING_LCONSONANTS = "bcdfghjklmnpqrstvwxyz";
      public const string STRING_QUOTES = @"'""";
      public const string STRING_TAB = "\t";
      public const string STRING_CRLF = "\r\n";
      public const string STRING_CR = "\r";
      public const string STRING_LF = "\n";

      public const string STRING_ACCENTS = "A`ÀA'ÁA^ÂA~ÃA:ÄA.ÅAEÆC,ÇE`ÈE'ÉE^ÊE:ËI`ÌI'ÍI^ÎI:ÏD-ÐN~ÑO`ÒO'ÓO^ÔO~ÕO:ÖO/" +
         "ØU`ÙU'ÚU^ÛU:ÜY'Ýa`àa'áa^âa~ãa:äa.åaeæc,çe`èe'ée^êe:ëi`ìi'íi^îi:ïn~ño`òo'óo^ôo~õo:öo/øu`ùu'úu^ûu:üy'ýy:ÿ";

      public const string STRING_BEGIN_PATTERN = "{"; //"“";
      public const string STRING_END_PATTERN = "}"; //"”";
      public const string STRING_TRUE = "True";

      public const string MESSAGE_BUILDER = "className";

      internal const string LOCATION = "Runtime";

      public static int WrapIndex(int index, int length, bool wrap)
      {
         Assert(length > -1, LOCATION, "Length can't be negative");
         if (length == 0)
            return 0;
         if (index < 0)
            return wrapNegativeIndex(index, length);

         return wrap ? (index >= length ? length % Math.Abs(index) : index) : index;
      }

      static int wrapNegativeIndex(int index, int length)
      {
         if (length == 0)
            return 0;

         var candidate = index + length;
         return candidate >= length ? length % Math.Abs(index) : candidate;
      }

      public static string Expand(string text)
      {
         if (text.IsEmpty())
            return "";

         var matcher = new Matcher();
         matcher.Evaluate(text, "/w ':' /w");
         for (var i = 0; i < matcher.MatchCount; i++)
            matcher[i] = matcher[i].IsMatch("^ /d") ? expandNumeric(matcher[i]) : expandAlpha(matcher[i]);

         var expanded = matcher.ToString();
         if (matcher.IsMatch(expanded, "/(/w+) '-' /(/w+)"))
         {
            var keep = matcher[0, 1];
            var remove = matcher[0, 2];
            keep = remove.Select((t, i) => remove.Substring(i, 1))
               .Aggregate(keep, (current, chr) => current.Replace(chr, ""));
            return keep;
         }

         return expanded;
      }

      static string expandAlpha(string text)
      {
         if (text.IsEmpty())
            return "";

         var matcher = new Matcher();
         matcher.Evaluate(text, "/(['a-zA-Z']) ':' /(['a-zA-Z'])");
         for (var i = 0; i < matcher.MatchCount; i++)
         {
            var first = (int)matcher[i, 1][0];
            var last = (int)matcher[i, 2][0];
            var result = new StringBuilder();
            for (var j = first; j <= last; j++)
               result.Append((char)j);

            matcher[i] = result.ToString();
         }

         return matcher.ToString();
      }

      static string expandNumeric(string text)
      {
         if (text.IsEmpty())
            return "";

         var matcher = new Matcher();
         matcher.Evaluate(text, "/(['0-9']) ':' /(['0-9'])");
         for (var i = 0; i < matcher.MatchCount; i++)
         {
            var first = matcher[i, 1].ToInt();
            var last = matcher[i, 2].ToInt();
            var result = new StringBuilder();
            for (var j = first; j <= last; j++)
               result.Append(j);

            matcher[i] = result.ToString();
         }

         return matcher.ToString();
      }

      public static bool InText(string subject, string character, StringComparison comparison) => subject.IndexOf(character, comparison) > -1;

      public static bool Find(string text, StringComparison comparison, int start, string needle, bool not, ref int index,
         ref int length)
      {
         for (var i = start; i < text.Length; i++)
         {
            var character = text.Skip(i).Take(1);
            if (character == "")
               return false;

            var inside = InText(needle, character, comparison);
            if (!not && !inside)
            {
               length = i - index;
               return length > 0;
            }

            if (!not || !inside)
               continue;

            length = i - index;
            return length > -1;
         }

         length = text.Length - index;
         return length > 0;
      }

      public static StringComparison SetUpSearchText(string text, bool ignoreCase, ref string needle)
      {
         var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
         needle = Expand(text);
         return comparison;
      }

      public static string FixText(string text, string input, ref string needle, ref char[] needleChars)
      {
         if (State.IgnoreCase)
         {
            text = text.ToLower();
            input = input.ToLower();
         }
         if (needle == null)
            needle = Expand(text);
         if (needleChars == null)
            needleChars = needle.ToCharArray();
         return input;
      }

      public static void Assert(bool test, string location, string message)
      {
         if (CanAssert)
            Assertions.Assert(test, message);
         else
            Assertions.Assert(test, $"at {location}: {message}");
      }

      public static void Assert(bool test, string location, Func<string> message)
      {
         if (CanAssert)
            Assertions.Assert(test, message);
         else
            Assertions.Assert(test, $"at {location}: {message}");
      }

      public static void Reject(bool test, string location, string message)
      {
         if (CanAssert)
            Assertions.Reject(test, message);
         else
            Assertions.Reject(test, $"at {location}: {message}");
      }

      public static void RejectNull(object obj, string location, string message)
      {
         if (CanAssert)
            Assertions.Reject(obj.IsNull(), message);
         else
            Assertions.Reject(obj.IsNull(), $"at {location}: {message}");
      }

      public static void Throw(string location, string message)
      {
         throw $"at {location}: {message}".Throws();
      }

      public static Value ConvertStringToNumber(Value source)
      {
         if (source.IsEmpty)
            return 0;

         var sourceText = source.Text;
         Parser parser = new FloatParser();
         var coloring = Parser.Coloring;
         Parser.Coloring = false;
         if (parser.Scan(sourceText, 0))
         {
            Parser.Coloring = coloring;
            return parser.Result.Value;
         }

         parser = new IntegerParser();
         if (parser.Scan(sourceText, 0))
         {
            Parser.Coloring = coloring;
            return parser.Result.Value;
         }

         Parser.Coloring = coloring;
         return 0;
      }

      public static string ReplaceEscapedValues(string text)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(text, "'`x_' /(['0-9a-f_']+)"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = getCharFromInt(HexParser.GetNumber(matcher[i, 1]));

            text = matcher.ToString();
         }

         if (matcher.IsMatch(text, "'`o_' /(['0-7_']+)"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = getCharFromInt(OctParser.GetNumber(matcher[i, 1]));

            text = matcher.ToString();
         }

         if (matcher.IsMatch(text, "'`b_' /(['01_']+)"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = getCharFromInt(BinParser.GetNumber(matcher[i, 1]));

            text = matcher.ToString();
         }

         if (matcher.IsMatch(text, "'`' /(['0-9_']+)"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
               matcher[i] = getCharFromInt(matcher[i, 1].Replace("_", "").ToInt());

            text = matcher.ToString();
         }

         return text;
      }

      static string getCharFromInt(int value) => ((char)value).ToString();

      public static bool IsNumeric(string value)
      {
         if (value == "0")
            return true;
         if (value.IsMatch("^ ['eE'] /d+ $"))
            return false;

         return value.IsNumeric() && !value.StartsWith("0") && !value.StartsWith("+") || value.IsFloat();
      }

      public static Value ConvertIfNumeric(Value value)
      {
         if (value == null)
            return "";

         switch (value.Type)
         {
            case Value.ValueType.Number:
               return value.Number;
            case Value.ValueType.String:
               return IsNumeric(value.Text) ? value.Number : value;
            default:
               return value;
         }
      }

      public static string GetReadableKey(string key) => key.IsNumeric() ? "$" + key : key;

      public static string GetReadableKey(Array.IterItem item) => GetReadableKey(item.Key);

      static Verb regularizeVerb(Verb verb)
      {
         if (verb is LessThan)
            verb = new BinaryLessThan();
         else if (verb is LessThanEqual)
            verb = new BinaryLessThanEqual();
         return verb;
      }

      public static IMaybe<(Block, bool)> OperatorBlock(Value value)
      {
         Verb verb;
         if (value.Type == Value.ValueType.Block)
         {
            var block = (Block)value;
            if (block.Count == 0)
               return none<(Block, bool)>();

            verb = regularizeVerb(block[0]);
         }
         else
         {
            var operatorText = value.Text;
            var type = TwoCharacterOperatorParser.Operator(operatorText);
            RejectNull(type, LOCATION, $"Didn't understand verb {operatorText}");
            var instance = (Verb)Activator.CreateInstance(type);
            verb = regularizeVerb(instance);
         }

         return OperatorBlock(verb);
      }

      public static IMaybe<(Block, bool)> OperatorBlock(Type type)
      {
         try
         {
            var verb = (Verb)Activator.CreateInstance(type);
            return OperatorBlock(verb);
         }
         catch
         {
            return none<(Block, bool)>();
         }
      }

      public static IMaybe<(Block, bool)> OperatorBlock(Verb verb)
      {
         var builder = new CodeBuilder();
         builder.Variable(DefaultParameterNames.VAR_VALUE1);
         builder.Verb(verb);
         builder.Variable(DefaultParameterNames.VAR_VALUE2);
         return (builder.Block, verb.LeftToRight).Some();
      }

      public static Value BitOperationOnText(Value x, Value y, Func<int, int, int> func)
      {
         var left = x.Text;
         var right = y.Text;
         var length = Math.Min(left.Length, right.Length);
         var result = new StringBuilder();
         for (var i = 0; i < length; i++)
         {
            var value = func(left[i], right[i]);
            result.Append((char)value);
         }

         return result.ToString();
      }

      public static object ConvertToObject(string value, bool isHex)
      {
         if (value == null)
            return null;

         if (value.IsQuoted())
            return value.ExtractFromQuotes();

         if (value.IsNumeric())
         {
            if (isHex)
               return value.ToInt();

            return value.ToDouble();
         }

         if (value.IsDate())
            return value.ToDateTime();
         if (value.Same("false") || value.Same("true"))
            return value.ToBool();

         return value;
      }

      public static Array GeneratorToArray(Value value) => value.PossibleGenerator().FlatMap(ToArray, () => new Array());

      public static Array ToArray(INSGenerator generator)
      {
         var array = new Array();
         var iterator = new NSIterator(generator);
         iterator.Reset();
         for (var i = 0; i < MAX_ARRAY; i++)
         {
            var next = iterator.Next();
            if (next.IsNil)
               break;

            array.Add(next);
         }

         return array;
      }

      public static Runtime State { get; set; }

      ValueStack stack;
      BlockManager blockManager;
      Buffer printBuffer;
      PatternManager patternManager;
      ExpressionManager expressionManager;
      int seed;
      Random random;
      Stack<ValueStack> valueStacks;
      Array takeArray;
      Stack<DefaultParameterNames> defaultParameterNames;
      Buffer buffer;
      Hash<string, IInvokeable> invokeables;
      Hash<string, InvokeableReference> extenders;
      ConsoleManager consoleManager;
      string indent;

      public Runtime(string text = "", IFileCache fileCache = null)
      {
         Sys = new Sys();
         stack = new ValueStack();
         blockManager = new BlockManager();
         printBuffer = new Buffer();
         patternManager = new PatternManager();
         Regions = new RegionManager(Sys, text);
         expressionManager = new ExpressionManager();
         FieldPattern = (Pattern)(STRING_BEGIN_PATTERN + "+" + STRING_END_PATTERN);
         RecordPattern = (Pattern)(STRING_BEGIN_PATTERN + "'`r`n' | '`r' | '`n'" + STRING_END_PATTERN);
         FieldSeparator = (String)" ";
         RecordSeparator = (String)"\r\n";
         seed = DateTime.Now.Millisecond;
         random = new Random(seed);
         valueStacks = new Stack<ValueStack>();
         FileCache = fileCache ?? new StandardFileCache();
         takeArray = new Array();
         defaultParameterNames = new Stack<DefaultParameterNames>();
         defaultParameterNames.Push(new DefaultParameterNames(true));
         buffer = new Buffer();
         ArgumentDepth = 0;
         invokeables = new Hash<string, IInvokeable>();
         extenders = new Hash<string, InvokeableReference>();
         Parser.InClassDefinition = false;
         consoleManager = new ConsoleManager();
         indent = "";
      }

      public Pattern RecordPattern { get; set; }

      public String RecordSeparator { get; set; }

      public Pattern FieldPattern { get; set; }

      public String FieldSeparator { get; set; }

      public Pattern ArrayPattern { get; set; }

      public String ArraySeparator { get; set; }

      public ValueStack Stack => stack;

      public IFileCache FileCache { get; set; }

      public IConsole UIConsole
      {
         get => consoleManager.UIConsole;
         set => consoleManager.UIConsole = value;
      }

      public ConsoleManager ConsoleManager => consoleManager;

      public void RegisterBlock(Block block, bool resolve = true)
      {
         blockManager.Register(block, resolve);
         Regions.Push("register block");
         expressionManager.Begin();
         PushValueStack();
      }

      public void RegisterBlock(Block block, Region region, bool resolve = true)
      {
         blockManager.Register(block, resolve);
         Regions.Push(region, "register block");
         expressionManager.Begin();
         PushValueStack();
      }

      public void UnregisterBlock()
      {
         PopValueStack();
         blockManager.Unregister();
         Regions.Pop("unregister block");
         expressionManager.End();
      }

      public Block Block => blockManager.Block;

      public void RegisterLambdaRegion(Region region) => blockManager.RegisterLambdaRegion(region);

      public void UnregisterLambdaRegion() => blockManager.UnregisterLambdaRegion();

      public Region LambdaRegion => blockManager.LambdaRegion;

      public void Print(string text) => printBuffer.Print(text);

      public void BuffPrint(string text) => buffer.Print(text);

      public void Put(string text) => printBuffer.Put(text);

      public void BuffPut(string text) => buffer.Put(text);

      public void Write(string text) => printBuffer.Write(text);

      public void BuffWrite(string text) => buffer.Write(text);

      public string PrintBuffer => printBuffer.ToString();

      public string Buffer => buffer.Result();

      public ConditionalReplacements Replacements => patternManager.Replacements;

      public int Position
      {
         get => patternManager.Position;
         set => patternManager.Position = value;
      }

      public bool Anchored
      {
         get => patternManager.Anchored;
         set => patternManager.Anchored = value;
      }

      public bool IgnoreCase
      {
         get => patternManager.IgnoreCase;
         set => patternManager.IgnoreCase = value;
      }

      public string Input
      {
         get => patternManager.Input;
         set => patternManager.Input = value;
      }

      public bool Aborted
      {
         get => patternManager.Aborted;
         set => patternManager.Aborted = value;
      }

      public bool FirstScan
      {
         get => patternManager.FirstScan;
         set => patternManager.FirstScan = value;
      }

      public Stack<AlternateData> Alternates => patternManager.Alternates;

      public string WorkingInput
      {
         get => patternManager.WorkingInput;
         set => patternManager.WorkingInput = value;
      }

      public void SaveWorkingInput() => patternManager.SaveWorkingInput();

      public void RestoreWorkingInput() => patternManager.RestoreWorkingInput();

      public void BindToPattern(string name, string value) => patternManager.Bind(name, value);

      public void AssignPatternBindings() => patternManager.AssignBindings();

      public bool Trace
      {
         get => patternManager.Trace;
         set => patternManager.Trace = value;
      }

      public void PushPatternManager() => patternManager.Push();

      public void PopPatternManager() => patternManager.Pop();

      public int PatternDepth { get; set; }

      public bool Multi
      {
         get => patternManager.Multi;
         set => patternManager.Multi = value;
      }

      public Value Result
      {
         get => patternManager.Result;
         set => patternManager.Result = value;
      }

      public static Array Array(string variableName)
      {
         if (Regions.ValueFromVariable(variableName).If(out var value))
         {
            if (value is Array array)
               return array;

            throw new ApplicationException($"at {LOCATION}: Variable doesn't refer to an array");
         }

         var newArray = new Array();
         Regions[variableName] = newArray;
         return newArray;
      }

      public int Random(int value) => random.Next(value);

      public double Random() => random.NextDouble();

      public int RandomInt() => random.Next();

      public Sys Sys { get; set; }

      public ExpressionManager Expressions => expressionManager;

      public void PushValueStack()
      {
         valueStacks.Push(stack);
         stack = new ValueStack();
      }

      public void PopValueStack() => stack = valueStacks.Pop();

      public Func<string> Asker { get; set; }

      public static Value SendMessage(Value value, string message) => SendMessage(value, message, new Arguments());

      public static Value SendMessage(Value value, string message, Value argumentValue) => SendMessage(value, message,
         new Arguments(argumentValue));

      public static Value SendMessage(Value value, string message, Arguments arguments) => MessageManager.MessagingState
         .SendMessage(value, message, arguments);

      public static Value SendMessage(Value value, Message message) => MessageManager.MessagingState
         .SendMessage(value, message.MessageName, message.MessageArguments);

      public bool ExitSignal { get; set; }

      public bool SkipSignal { get; set; }

      public Value ReturnValue { get; set; }

      public bool ReturnSignal { get; set; }

      public Block LateBlock { get; set; }

      public Value UseReturnValue(Value result)
      {
         if (ReturnSignal)
         {
            ReturnSignal = false;
            var returnResult = ReturnValue;
            ReturnValue = null;
            return stripFromThunk(returnResult);
         }

         if (ResultValue != null)
         {
            var resultValue = ResultValue;
            ResultValue = null;
            return stripFromThunk(resultValue);
         }

         return stripFromThunk(result);
      }

      static Value stripFromThunk(Value value) => value is Thunk thunk ? thunk.AlternateValue("return") : value;

      public void Give(Value value) => takeArray.Add(value);

      public Array Take()
      {
         var result = (Array)takeArray.Clone();
         takeArray = new Array();
         return result;
      }

      public bool Tracing { get; set; }

      public DefaultParameterNames PushDefaultParameterNames()
      {
         var parameterNames = new DefaultParameterNames();
         defaultParameterNames.Push(parameterNames);
         return parameterNames;
      }

      public DefaultParameterNames PushUpperLevelParameterNames()
      {
         var parameterNames = defaultParameterNames.Peek().Clone();
         defaultParameterNames.Push(parameterNames);
         return parameterNames;
      }

      public DefaultParameterNames DefaultParameterNames => defaultParameterNames.Peek();

      public void PopDefaultParameterNames() => defaultParameterNames.Pop();

      public void ClearDefaultParameterNames()
      {
         defaultParameterNames.Clear();
         var parameterNames = new DefaultParameterNames(true);
         defaultParameterNames.Push(parameterNames);
      }

      public static Message MessageFromArguments(Arguments arguments)
      {
         var value = arguments[0];
         if (value is Message message)
            return message;

         var variable = arguments.VariableName(0);
         if (variable.IsNotEmpty())
            return new Message(variable, new Arguments());

         var messageName = value.Text;
         return messageName.IsNotEmpty() ? new Message(messageName, new Arguments()) : null;
      }

      public int ArgumentDepth { get; set; }

      public Value ResultValue { get; set; }

      public static bool IsXMethodAvailable(string message)
      {
         var variableName = Format(VAR_XMETHOD, message);
         return Regions.VariableExists(variableName) && Regions[variableName].IsTrue;
      }

      public static IMaybe<IXMethod> XMethodAvailable(string message) => Regions[message].IfCast<IXMethod>();

      public static void MarkAsXMethod(string message, IXMethod xMethod)
      {
         if (!xMethod?.XMethod != true)
            return;

         var variableName = Format(VAR_XMETHOD, message);
         Regions.CreateVariable(variableName, true, _override: true);
         Regions[variableName] = true;
      }

      public static Value InvokeXMethod(string message, Object obj, Arguments arguments)
      {
         var xMethod = XMethodAvailable(message);
         if (xMethod.IsSome)
         {
            var newArguments = arguments.Clone();
            newArguments.Unshift(obj);
            return SendMessage((Value)xMethod.Value, "invoke", newArguments);
         }

         return null;
      }

      public void SetInvokeable(string name, IInvokeable invokeable) => invokeables[name] = invokeable;

      public IInvokeable GetInvokeable(string name)
      {
         var invokeable = invokeables[name];
         if (invokeable == null)
         {
            Throw(LOCATION, $"Invokeable reference {name} not found");
            return null;
         }

         return invokeable;
      }

      public void SetExtender(string className, string messageName, InvokeableReference reference)
      {
         var key = extenderKey(className, messageName);
         extenders[key] = reference;
      }

      static string extenderKey(string className, string messageName) => $"{className}/{messageName}";

      public InvokeableReference GetExtender(string className, string messageName)
      {
         var key = extenderKey(className, messageName);
         return extenders[key];
      }

      public bool CanExtend(string className, string messageName)
      {
         var key = extenderKey(className, messageName);
         return extenders.ContainsKey(key);
      }

      public static int Compare(Value x, Value y)
      {
         if (x.Type == Value.ValueType.Object && ((Object)x).RespondsNoDefault("cmp"))
            return (int)SendMessage(x, "cmp", y).Number;

         return x.Compare(y);
      }

      public static string Text(Value x)
      {
         if (x.Type == Value.ValueType.Object && ((Object)x).RespondsNoDefault("str"))
            return SendMessage(x, "str").Text;

         return x.Text;
      }

      public Value Seed
      {
         get => seed;
         set
         {
            seed = (int)value.Number;
            random = new Random(seed);
         }
      }

      public static string LongToMangledPrefix(string type, string name)
      {
         switch (type)
         {
            case "get":
               return $"__$get_{name}";
            case "set":
               return $"__$set_{name}";
            case "before":
               return $"__$bef_{name}";
            case "after":
               return $"__$aft_{name}";
            case "require":
               return $"__$req_{name}";
            case "ensure":
               return $"__$ens_{name}";
            case "invariant":
               return $"__$inv_{name}";
            default:
               return name;
         }
      }

      public static string ShortToMangledPrefix(string type, string name)
      {
         switch (type)
         {
            case "get":
               return $"__$get_{name}";
            case "set":
               return $"__$set_{name}";
            case "bef":
               return $"__$bef_{name}";
            case "aft":
               return $"__$aft_{name}";
            case "req":
               return $"__$req_{name}";
            case "ens":
               return $"__$ens_{name}";
            case "inv":
               return $"__$inv_{name}";
            default:
               return name;
         }
      }

      public static string MangledName(string name) => $"{VAR_MANGLE}{name}";

      public static string GetterName(string name) => MangledName($"get_{name}");

      public static string SetterName(string name) => MangledName($"set_{name}");

      public static string Unmangle(string name)
      {
         return name.Matches("^ '__$' /(/w+) '_' /(/w+)").FlatMap(m => $"{m.FirstGroup} {m.SecondGroup}", () => name);
      }

      public static bool IsPrefixed(string name, out string type, out string plainName)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(name, "^ '__$' /(/w+) '_' /@"))
         {
            type = matcher[0, 1];
            plainName = matcher[0, 2];
            return true;
         }

         type = "";
         plainName = "";
         return false;
      }

      public static bool IsPrefixed(string name, string type) => name.IsMatch($@"^ '__$'{type}'_'");

      public static void ExecuteWhere(IWhere where)
      {
         var block = where.Where;
         if (block == null)
            return;

         block.AutoRegister = false;
         block.Evaluate();
      }

      public void Indent(int count) => indent = "\t".Repeat(count);

      public void IndentBy(int count)
      {
         if (count > 0)
            indent += "\t".Repeat(count);
         else if (count < 0 && indent.IsNotEmpty())
            indent = indent.Skip(count);
      }

      public string Indentation() => indent;

      public FolderName[] ModuleFolders { get; set; } = new FolderName[0];

      public static bool IsClassName(string name) => char.IsUpper(name[0]);
   }
}