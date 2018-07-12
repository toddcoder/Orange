using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Defer : Verb
   {
      Block expression;
      public Defer(Block expression) => this.expression = expression;

      public override Value Evaluate()
      {
         var value = expression.Evaluate();
         State.ResultValue = value;
         return value;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"defer {expression}";
   }
}