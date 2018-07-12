using System.Collections.Generic;
using System.IO;
using System.Linq;
using Orange.Library.Parsers;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Booleans;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Compiler;
using static Orange.Library.Debugging.Debugger;
using static Orange.Library.Parsers.Parser;
using static Orange.Library.Runtime;

namespace Orange.Library
{
   public class OrangeCompiler
   {
      public enum ConsumeEndOfBlockType
      {
         Consume,
         Leave,
         SingleLine
      }

/*      public static Tuple<Block, int> ParseBlock(string source, int index, string endOfBlock,
         ConsumeEndOfBlockType consumeEndOfBlock = Consume)
      {
         var consume = true;
         switch (consumeEndOfBlock)
         {
            case Leave:
               consume = false;
               break;
            case SingleLine:
               consume = PreviousEndings.Count == 0 || PreviousEndings.Peek().DefaultTo(() => "") != REGEX_END;
               break;
         }
         PreviousEndings.Push(endOfBlock);
         var compiler = new OrangeCompiler(source, index, endOfBlock, consume);
         var block = compiler.Compile();
         PreviousEndings.Pop();
         return tuple(block, compiler.Position);
      }*/

      string source;
      int lastPosition;
      int initialPosition;
      int sourceLength;
      StringWriter verboseText;
      string endOfBlock;
      bool consumeEndOfBlock;
      bool inIf;

      public OrangeCompiler(string source, int initialPosition = 0, string endOfBlock = "", bool consumeEndOfBlock = true,
         bool inIf = false)
      {
         this.source = source;
         sourceLength = this.source.Length;
         this.initialPosition = initialPosition;
         if (initialPosition == 0)
            CompilerState = new Compiler();
         verboseText = new StringWriter();
         this.endOfBlock = endOfBlock;
         this.consumeEndOfBlock = consumeEndOfBlock;
         this.inIf = inIf;
         if (initialPosition == 0)
         {
            InClassDefinition = false;
            InStatic = false;
            PreviousEndings.Clear();
         }
      }

      public Block Compile()
      {
         var position = initialPosition;
         var previousPosition = initialPosition;
         InitialPosition = initialPosition;
         var block = new Block();
         var scanning = true;
         var yieldCount = 0;
         var parsers = new List<Parser>
         {
            new MultilineCommentParser(),
            new SingleLineCommentParser(),
            new NoEndParser(),
            new EndBlockParser(endOfBlock, consumeEndOfBlock),
            new EndParser(endOfBlock == REGEX_END),
            //new AnonymousObjectParser(),
            new SomeParser(),
            new ClassParser(),
            new TypeParser(),
            new StaticParser(),
            new TraitParser(),
            new NullOpParser(),
            new HexParser(),
            new BinParser(),
            new OctParser(),
            new FloatParser(),
            new IntegerParser(),
            new StringParser(),
            new FillInBlockParser(),
            new InterpolatedStringParser(),
            new ArrayLiteralParser(),
            //new ArrayParametersParser(),
            new DateParser(),
            //new IndexParser(),
            new TypeNameParser(),
            new SpecialValueParser(),
            new ChangeSignParser(),
            new PreIncrementDecrementParser(),
            new ReadOnlyParser(),
            new ReferenceParser(),
            new SymbolParser(),
            new SendMessageParser(),
            //new MessageBlockParser(),
            new AbstractParser(),
            new AutoPropertyParser(),
            new InitBlockParser(),
            new LetParser(),
            new MacroParser(),
            new OperatorParser(),
            new ListParser(),
            new FunctionParser(),
            new DefineExpressionParser(),
            new NewObjectParser(),
            new FunctionInvokeParser(),
            new DefineParser(),
            new EnumerationValueParser(),
            new DelegateParser(),
            new PlainSignalParser(),
            //new ScalarParser(),
            new IsDefinedParser(),
            //new ApplyInvokeParser(),
            new RegexParser(),
            new MessageArgumentParser(),
            //new AlternateLambdaParser(),
            new MacroLiteralParser(),
            new DeferredParser(),
            new ThreeCharacterOperatorParser(),
            new TwoCharacterOperatorParser(),
            new OneCharacterOperatorParser(),
            new InvokeParser(),
            new PrintBlockParser(),
            new GeneratorParser(),
            new EmptyArrayLiteralParser(),
            //new BlockParser(true),
            new PatternParser(),
            new IndexerParser(),
            new FormatLiteralParser(),
            new LazyBlockParser(),
            new IfParser(),
            new MatchParser(),
            new WhileParser(),
            new WithParser(),
            new WhereParser(),
            new CForParser(),
            new AssertParser(),
            new ShortCircuitBooleanParser(),
            new DisjoinMessageParser(),
            new WordOperatorParser(),
            new VariableParser()
         };

         while (position < sourceLength && scanning)
         {
            if (Verbose)
               verboseText.WriteLine($"Scanning {source.Skip(position)}");
            scanning = false;
            var found = false;
            foreach (var parser in parsers.Where(p => p.Scan(source, position)))
            {
               var result = parser.Result;
               if (result.Verb != null && !(result.Verb is NullOp))
               {
                  if (result.Verb is Yield)
                     yieldCount++;
                  block.Add(result.Verb);
                  if (result.Next != null)
                  {
                     block.Add(result.Next);
                     result.Next = null;
                  }
                  if (Verbose)
                     verboseText.WriteLine($"   Found {parser.VerboseName}");
               }
               if (result.Verbs != null && result.Verbs.Count > 0)
               {
                  foreach (var verb in result.Verbs)
                  {
                     if (verb is Yield)
                        yieldCount++;
                     block.Add(verb);
                  }
                  result.Verbs.Clear();
                  if (Verbose)
                     verboseText.WriteLine($"   Found {parser.VerboseName}");
               }
               position = result.Position;
               (position != -1).Assert("-1?");
               scanning = !parser.EndOfBlock;
               found = true;
               break;
            }
            var substring = source.Substring(position);
            found.Assert($"Orange compiler: Didn't understand \"{substring.Substitute("/s+", " ").TrimAll()}\"");
            (position != previousPosition || !scanning).Assert("Orange compiler: Scanning position has not changed");
            previousPosition = position;
         }
         (initialPosition > 0 && scanning && endOfBlock != REGEX_END).Reject("End of block not found");
         if (IsDebugging && initialPosition == 0)
            DebuggingState.Final();
         lastPosition = position;
         if (yieldCount > 1)
         {
            var blockGenerator = new YieldGenerator(block);
            block = PushValue(blockGenerator);
            var generator = new Generator(VAR_MANGLE + "b", block);
            block = PushValue(generator);
         }
         block.YieldCount = yieldCount;
         return block;
      }

      public int Position => lastPosition;

      public bool Verbose
      {
         get;
         set;
      }

      public string VerboseText => verboseText.ToString();
   }
}