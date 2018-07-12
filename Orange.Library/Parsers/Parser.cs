using System.Collections.Generic;
using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.DataStructures;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Debugging.Debugger;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public abstract class Parser
   {
      static Hash<string, IDEColor> ideColors;
      static int nextPosition;
      static string lastKey;
      static List<Block> staticBlocks;

      static Parser()
      {
         ideColors = new Hash<string, IDEColor>();
         nextPosition = 0;
         lastKey = "";
         Coloring = true;
         staticBlocks = new List<Block>();
      }

      public static bool Coloring { get; set; }

      public static Hash<string, IDEColor> IDEColors => ideColors;

      public static void Color(int position, int length, EntityType type)
      {
         if (Coloring)
         {
            var ideColor = new IDEColor { Position = position, Length = length, Type = type };
            lastKey = ideColor.Key;
            ideColors[lastKey] = ideColor;
            nextPosition = position + length;
         }
      }

      public static void Color(int length, EntityType type)
      {
         if (Coloring)
         {
            var ideColor = new IDEColor { Position = nextPosition, Length = length, Type = type };
            lastKey = ideColor.Key;
            ideColors[lastKey] = ideColor;
            nextPosition += length;
         }
      }

      public static IDEColor LastColor => ideColors[lastKey];

      public static int InitialPosition { get; set; }

      public static void AddStaticBlock(Block block) => staticBlocks.Add(block);

      public static Block GetStaticBlock()
      {
         var staticBlock = new Block();
         foreach (var block in staticBlocks)
         {
            foreach (var verb in block.AsAdded)
               staticBlock.Add(verb);

            staticBlock.Add(new End());
         }

         staticBlocks.Clear();
         return staticBlock;
      }

      public static bool InClassDefinition { get; set; }

      public static bool InStatic { get; set; }

      public static int EnumerationValue { get; set; }

      public static string ClassName { get; set; }

      public static CodeBuilder EnumerationMappingCode { get; set; }

      public static bool LockedDown { get; set; }

      public static List<CreateFunction> HelperFunctions { get; set; }

      public static Block HelperBlock { get; set; }

      public static Object.VisibilityType CurrentVisibility { get; set; } = Object.VisibilityType.Public;

      public static string EndStructure(string beginStructure)
      {
         switch (beginStructure)
         {
            case "(":
               return "')'";
            case "[":
               return "']'";
            case "{":
               return "'}'";
            case "<":
               return "'>'";
            default:
               return beginStructure;
         }
      }

      public static MaybeStack<string> PreviousEndings { get; set; } = new MaybeStack<string>();

      public static string Tabs { get; set; } = "";

      public static void AdvanceTabs() => Tabs = Tabs + "/t";

      public static void RegressTabs() => Tabs = Tabs.Skip(-2);

      public static IMaybe<int> ConsumeEndOfLine(string source, int index)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(source.Skip(index), "^ /r /n | /r | /n"))
         {
            var matchedLength = matcher.Length;
            Color(index, matchedLength, Whitespaces);
            return (index + matchedLength).Some();
         }

         return none<int>();
      }

      protected string pattern;
      protected bool ignoreCase;
      protected bool multiline;
      protected ParserResult result;
      protected string source;
      protected int position;
      protected int? overridePosition;
      protected int length;

      public Parser(string pattern, bool ignoreCase = false, bool multiline = false)
      {
         this.pattern = pattern;
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         result = new ParserResult { Verb = null, Position = 0 };
      }

      static string replaceInPattern(string pattern)
      {
         if (Tabs == "/s*")
            return pattern.Replace("|tabs|", "/s*").Replace("|tabs1|", "").Replace("|sp|", "/s*")
               .Replace("|sp+|", "[' ' /t]+");

         return pattern.Replace("|tabs|", Tabs).Replace("|tabs1|", Tabs.Skip(-2)).Replace("|sp|", "/s*")
            .Replace("|sp+|", "[' ' /t]+");
      }

      public virtual bool Scan(string source, int position)
      {
         result.Verb = null;
         result.Position = -1;
         var newPattern = replaceInPattern(pattern);

         var matcher = new Matcher();
         if (matcher.IsMatch(source.Substring(position), newPattern, ignoreCase, multiline))
         {
            this.source = matcher[0];
            this.position = position;
            this.source = source;
            overridePosition = null;
            var tokens = matcher.Groups(0);
            length = tokens[0].Length;
            var verb = CreateVerb(tokens);
            if (verb == null)
               return false;

            result.Verb = verb;
            result.Position = overridePosition ?? position + length;
            if (IsDebugging)
            {
               var lineLength = overridePosition - position ?? length;
               var line = source.Substring(position, lineLength);
               if (result.Verb is IEnd)
                  DebuggingState.EndSource(line);
               else
                  DebuggingState.AddSource(line);
               result.Verb.LineNumber = DebuggingState.LineNumber;
               result.Verb.LinePosition = DebuggingState.LinePosition;
            }
            return true;
         }

         FailedScan(source, position);
         if (NextParser(source, position, out var nextResult))
         {
            result = nextResult;
            return true;
         }

         return false;
      }

      public abstract Verb CreateVerb(string[] tokens);

      public virtual void FailedScan(string source, int position) { }

      public virtual bool NextParser(string source, int position, out ParserResult result)
      {
         result = new ParserResult();
         return false;
      }

      public virtual ParserResult Result => result;

      public virtual bool EndOfBlock => false;

      public override string ToString() => pattern;

      public abstract string VerboseName { get; }

      public int Length => length;
      public Verb Verb => result.Verb;
      public Value Value => result.Value;
      public int Position => result.Position;

      protected bool beginningOfBlock() => position == 0 || source.Substring(position - 1).IsMatch("^ ['{,[(']");

      public int NextPosition => position + Length;

      protected EndOfLineType matchEndOfLine()
      {
         if (NextPosition < source.Length)
         {
            var endOfLineParser = new EndOfLineParser();
            if (endOfLineParser.Scan(source, NextPosition))
            {
               overridePosition = endOfLineParser.Position;
               return EndOfLineType.EndOfLine;
            }

            return EndOfLineType.More;
         }

         return EndOfLineType.EndOfSource;
      }
   }
}