using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class StatementParser : MultiParser
   {
      [Flags]
      public enum InclusionType
      {
         InClass = 0,
         Classes = 1
      }

      public static IMaybe<(Block, int)> GetBlock(string source, int index, bool advanceTabs,
         InclusionType inclusion = InclusionType.Classes, bool compileAll = false)
      {
         if (advanceTabs)
            AdvanceTabs();

         var whenUnlessParser = new WhenUnlessParser();
         var statementParser = new StatementParser { Inclusions = inclusion };
         var block = new Block();
         var continuing = true;

         while (continuing && index < source.Length)
         {
            continuing = false;
            foreach (var parserResult in statementParser.Parsers
               .Where(parser => parser.Scan(source, index))
               .Select(parser => parser.Result))
            {
               whenUnlessParser.OtherVerb = parserResult.Verb;
               if (whenUnlessParser.Scan(source, parserResult.Position))
               {
                  block.Add(whenUnlessParser.Verb);
                  index = whenUnlessParser.Position;
               }
               else
               {
                  block.Add(parserResult.Verb);
                  index = parserResult.Position;
               }
               continuing = true;
               break;
            }

            if (continuing && index < source.Length)
            {
               var endParser = new EndParser();
               if (endParser.Scan(source, index))
                  index = endParser.Position;
            }
         }

         if (compileAll)
            Assert(index >= source.Length, "Compilation", $"Didn't understand {source.Skip(index)}");

         if (advanceTabs)
            RegressTabs();

         return when(block.Count > 0, () => (block, index));
      }

      public static IMaybe<(Block, int)> OneLineStatement(string source, int index)
      {
         var oldTabs = Tabs;
         Tabs = "/s*";
         var whenUnlessParser = new WhenUnlessParser();

         try
         {
            var parser = new StatementParser();
            if (parser.Scan(source, index))
            {
               var verb = ReturnExpression.Convert(parser.Verb);
               whenUnlessParser.OtherVerb = verb;
               if (whenUnlessParser.Scan(source, parser.Position))
                  return (Inline(whenUnlessParser.Verb), whenUnlessParser.Position).Some();

               return (Inline(verb), parser.Position).Some();
            }

            return none<(Block, int)>();
         }
         finally
         {
            Tabs = oldTabs;
         }
      }

      public static IMaybe<(Block, int)> GetOneOrMultipleBlock(string source, int index, string stopPattern = " ^ /s * ':' /s+")
      {
         var parser = new FreeParser();
         if (parser.Scan(source, index, "^ ' '* 'then' /b"))
            return none<(Block, int)>();

         if (parser.Scan(source, index, stopPattern))
         {
            parser.ColorAll(Structures);
            return OneLineStatement(source, parser.Position);
         }

         var oneOrMultipleBlock = GetBlock(source, index, true);
         Assert(oneOrMultipleBlock.IsSome, "Statement parser", "Couldn't determine block");
         return oneOrMultipleBlock;
      }

      EndOfLineParser endOfLineParser;

      public StatementParser() => endOfLineParser = new EndOfLineParser();

      public Bits32<InclusionType> Inclusions { get; set; } = InclusionType.Classes;

      public override string VerboseName => "statements";

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            //yield return new EndParser();
            yield return new BlankLineParser();
            yield return new MultilineCommentParser();
            yield return new SingleLineCommentParser();
            yield return new ClassParser();
            yield return new TraitParser();
            yield return new TypeParser();
            yield return new DataParser();
            yield return new EnumerationValueParser();
            yield return new AbstractParser();
            yield return new IndexedSetterMessageParser();
            yield return new IndexedSetterParser();
            yield return new ClassSetterParser();
            yield return new StaticParser();
            yield return new FunctionParser();
            yield return new PseudoRecursionParser();
            yield return new DefineExpressionParser();
            yield return new PlainSignalParser();
            yield return new IfParser();
            yield return new IndentParser();
            yield return new MaybeParser();
            yield return new GuardParser();
            yield return new MatchParser();
            yield return new WhileParser();
            yield return new WithParser();
            yield return new ImportParser();
            yield return new TryParser();
            yield return new HideParser();
            yield return new PublishParser();
            yield return new InheritParser();
            yield return new RepeatParser();
            yield return new ForParser();
            yield return new IterateParser();
            yield return new UseParser();
            yield return new AssertParser();
            yield return new StopParser();
            yield return new MultiAssignParser();
            yield return new AssignToNewFieldParser();
            yield return new AssignToFieldParser();
            yield return new LetParser();
            yield return new CreateFieldParser();
            yield return new ReturnSignalParser();
            yield return new YieldParser();
            yield return new DeferParser();
            yield return new PrintlnParser();
            yield return new PrintParser();
            yield return new DoNothingParser();
            yield return new ExpressionLineParser();
            yield return new EndOfLineParser();
         }
      }

      public override bool Continue(Parser parser, string source)
      {
         if (result.Position >= source.Length)
            return true;

         if (endOfLineParser.Scan(source, result.Position))
            overridePosition = endOfLineParser.Result.Position;
         return true;
      }
   }
}