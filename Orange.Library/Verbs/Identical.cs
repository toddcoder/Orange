using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class Identical : Verb
   {
      const string LOCATION = "Identical";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         return x.ID == y.ID;
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Equals;

      public override string ToString() => "===";
   }
}