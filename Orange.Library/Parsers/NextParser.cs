using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;
using If = Orange.Library.Values.If;

namespace Orange.Library.Parsers
{
   public class NextParser : Parser, IReturnsBlock
   {
      static (Verb, int) enclose(bool forward, Block condition, Block block, string source, int index)
      {
         var newCondition = forward ? condition : CodeBuilder.Not(condition);
         var elseParser = new ElseParser();
         var _if = new If(newCondition, block);
         var overriding = index;
         if (elseParser.Scan(source, index))
         {
            _if.ElseBlock = elseParser.Block;
            overriding = elseParser.Position;
         }
         var ifExecute = new IfExecute(_if);
         return (ifExecute, overriding);
      }

      public NextParser()
         : base("^ |tabs| 'next' /b (/s+ /('while' | 'until') /s+)?") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var op = tokens[1].Trim();

         Color(position, length, KeyWords);
         var forward = none<bool>();
         var condition = none<(Block, int)>();
         switch (op)
         {
            case "while":
               forward = true.Some();
               break;
            case "until":
               forward = false.Some();
               break;
         }

         var index = NextPosition;

         if (forward.IsSome)
         {
            condition = GetExpression(source, index, EndOfLineConsuming());
            if (condition.IsSome)
               index = condition.Value.Item2;
            else
               return null;
         }

         if (GetBlock(source, index, true).If(out var block, out var i))
         {
            int overriding;
            if (forward.IsSome)
            {
               (var ifVerb, var j) = enclose(forward.Value, condition.Value.Item1, block, source, i);
               overriding = j;
               block = new Block { ifVerb };
            }
            else
               overriding = i;
            Block = block;
            overridePosition = overriding;
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "next";

      public Block Block { get; set; }
   }
}