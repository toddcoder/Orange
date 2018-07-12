using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class Swap : Verb
   {
      const string LOCATION = "Swap";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var variable2 = stack.Pop<Variable>(false, LOCATION);
         var variable1 = stack.Pop<Variable>(false, LOCATION);
         var value1 = variable1.Value;
         var value2 = variable2.Value;
         variable1.Value = value2;
         variable2.Value = value1;
         return variable1;
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Statement;

      public override bool LeftToRight => false;

      public override string ToString() => "<->";
   }
}