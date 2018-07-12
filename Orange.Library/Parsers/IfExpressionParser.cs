using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using If = Orange.Library.Values.If;

namespace Orange.Library.Parsers
{
   public class IfExpressionParser : Parser
   {
      public IfExpressionParser()
         : base("^ /(|sp|) /'if' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);

         if (GetExpression(source, NextPosition, Stop.IfThen()).If(out var condition, out var i) &&
            GetExpression(source, i, Stop.IfElse()).If(out var ifTrue, out var j) &&
            GetExpression(source, j, Stop.IfEnd()).If(out var ifFalse, out var k))
         {
            overridePosition = k;
            var ifResult = new If(condition, ifTrue) { ElseBlock = ifFalse };
            return new IfExecute(ifResult, VerbPresidenceType.Push);
         }

         return null;
      }

      public override string VerboseName => "if expression";
   }
}