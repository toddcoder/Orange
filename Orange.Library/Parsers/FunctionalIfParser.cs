using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using If = Orange.Library.Values.If;

namespace Orange.Library.Parsers
{
   public class FunctionalIfParser : Parser
   {
      public FunctionalIfParser()
         : base("^ |sp| '(?'") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);

         if (GetExpression(source, NextPosition, FuncThen()) is Some<(Block, int)> e1)
         {
            (var condition, var i) = e1.Value;
            if (GetExpression(source, i, FuncThen()) is Some<(Block, int)> e2)
            {
               (var thenExpression, var j) = e2.Value;
               if (GetExpression(source, j, FuncEnd()) is Some<(Block, int)> e3)
               {
                  (var elseExpression, var k) = e3.Value;
                  overridePosition = k;
                  var _if = new If(condition, thenExpression) { ElseBlock = elseExpression };
                  return new IfExecute(_if, VerbPresidenceType.Push);
               }
            }
         }

         return null;
      }

      public override string VerboseName => "functional if";
   }
}