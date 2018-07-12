using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class PushGraphToAutoAssign : Verb
   {
      public override Value Evaluate()
      {
         var value = Runtime.State.Stack.Pop(true, "Push graph to $auto-assign");
         return new Graph(Runtime.VAR_AUTOASSIGN, value);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.PushGraph;

      public override string ToString() => "<--";

      public override bool LeftToRight => false;
   }
}