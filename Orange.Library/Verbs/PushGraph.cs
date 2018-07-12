using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class PushGraph : Verb
   {
      const string LOCATION = "Push graph";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var value = stack.Pop(true, LOCATION, false);
         var nameValue = stack.Pop(false, LOCATION);
         var variable = nameValue as Variable;
         var name = variable == null ? nameValue.Text : variable.Name;
         return new Graph(name, value);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.PushGraph;

      public override string ToString() => "<-";

      public override bool LeftToRight => false;
   }
}