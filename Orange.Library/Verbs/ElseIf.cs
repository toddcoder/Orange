using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class ElseIf : Verb
   {
      const string LOCATION = "else if";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var result = stack.Pop(true, LOCATION, false);
         var condition = stack.Pop(true, LOCATION);
         return new Values.When(condition, result);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.When;

      public override string ToString() => "elseif";
   }
}