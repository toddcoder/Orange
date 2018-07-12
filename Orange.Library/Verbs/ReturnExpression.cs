using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class ReturnExpression : Verb
   {
      public static Verb Convert(Verb verb) => verb is EvaluateExpression v ? new ReturnExpression(v.Block) : verb;

      Block expression;

      public ReturnExpression(Block expression)
      {
         this.expression = expression;
         this.expression.AutoRegister = false;
      }

      public override Value Evaluate() => expression.Evaluate();

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => expression.ToString();
   }
}