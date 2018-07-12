using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.StatementParser;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class ExpressionParser : NullParser
   {
      public const string REGEX_END_OF_LINE = "(^ /r /n) | (^ /r) | (^ /n) | (^ $)";
      public const string REGEX_DO_OR_END = "^ ' '* ('do' /b | " + REGEX_END_OF_LINE + ")";

      public static IMaybe<(Block, int)> GetExpression(string source, int index, Stop stop, bool asStatement = false)
      {
         var parser = new ExpressionParser(stop, asStatement);
         return when(parser.Scan(source, index), () => (parser.Block, parser.Position));
      }

      public static IMaybe<(Block, Block, int)> GetExpressionThenBlock(string source, int index)
      {
         return GetExpression(source, index, Stop.ExpressionThenBlock())
            .Map(t => GetOneOrMultipleBlock(source, t.Item2).Map(t2 => (t.Item1, t2.Item1, t2.Item2)));
      }

      Stop stop;
      bool asStatement;
      PrefixOperatorParser prefixOperatorParser;
      ValueParser valueParser;
      PostfixOperatorParser postfixOperatorParser;
      InfixOperatorParser infixOperatorParser;
      FreeParser freeParser;
      AndOrParser andOrParser;
      IgnoreReturnsParser ignoreReturnsParser;
      IfExpressionParser ifExpressionParser;

      public ExpressionParser(Stop stop, bool asStatement)
      {
         this.stop = stop;
         this.asStatement = asStatement;

         freeParser = new FreeParser();
      }

      IMaybe<int> isStopping(int index)
      {
         if (freeParser.Scan(source, index, stop.Pattern))
         {
            if (stop.Consume)
            {
               freeParser.ColorAll(stop.Color);
               return freeParser.Position.Some();
            }

            return index.Some();
         }

         return none<int>();
      }

      public override Verb Parse()
      {
         var block = new Block();
         var index = position;
         prefixOperatorParser = new PrefixOperatorParser();
         valueParser = new ValueParser();
         postfixOperatorParser = new PostfixOperatorParser(asStatement);
         infixOperatorParser = new InfixOperatorParser();
         andOrParser = new AndOrParser(Stop.PassAlong(stop, Structures));
         ignoreReturnsParser = new IgnoreReturnsParser();
         ifExpressionParser = new IfExpressionParser();

         IMaybe<int> newIndex;

         if (index < source.Length)
         {
            newIndex = isStopping(index);
            if (newIndex.IsSome)
               return returnBlock(block, newIndex.Value);

            newIndex = getTerm(block, index);
            if (newIndex.IsNone || newIndex.Value == index)
               return null;

            index = newIndex.Value;
         }

         while (index < source.Length)
         {
            newIndex = isStopping(index);
            if (newIndex.IsSome)
               return returnBlock(block, newIndex.Value);

            if (infixOperatorParser.Scan(source, index))
            {
               block.Add(infixOperatorParser.Verb);
               index = infixOperatorParser.Position;
            }
            else
               break;

            newIndex = getTerm(block, index);
            if (newIndex.IsNone || newIndex.Value == index)
               return null;

            index = newIndex.Value;
         }

         if (index < source.Length)
         {
            newIndex = isStopping(index);
            if (newIndex.IsSome)
               return returnBlock(block, newIndex.Value);

            if (andOrParser.Scan(source, index))
            {
               block.Add(andOrParser.Verb);
               index = andOrParser.Position;
            }
         }

         return returnBlock(block, index);
      }

      Verb returnBlock(Block block, int index)
      {
         block.Expression = true;
         block.Yielding = false;
         result.Value = block;
         Block = block;
         overridePosition = index;

         return new Push(block);
      }

      IMaybe<int> getTerm(Block block, int index)
      {
         index = ignoreReturns(index);

         if (ifExpressionParser.Scan(source, index))
         {
            block.Add(ifExpressionParser.Verb);
            return ifExpressionParser.Position.Some();
         }

         while (prefixOperatorParser.Scan(source, index))
         {
            block.Add(prefixOperatorParser.Verb);
            index = prefixOperatorParser.Position;
         }

         index = ignoreReturns(index);

         if (valueParser.Scan(source, index))
         {
            block.Add(valueParser.Verb);
            index = valueParser.Position;
         }
         else
            return none<int>();

         index = ignoreReturns(index);

         while (postfixOperatorParser.Scan(source, index))
         {
            block.Add(postfixOperatorParser.Verb);
            index = postfixOperatorParser.Position;
            index = ignoreReturns(index);
         }

         return index.Some();
      }

      int ignoreReturns(int index)
      {
         if (ignoreReturnsParser.Scan(source, index))
            index = ignoreReturnsParser.Position;
         return index;
      }

      public Block Block { get; set; }
   }
}