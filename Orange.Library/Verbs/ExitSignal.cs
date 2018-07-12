using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class ExitSignal : Verb, IEnd
   {
      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         if (stack.IsEmpty)
         {
            Runtime.State.ExitSignal = true;
            return null;
         }
         var value = stack.Pop(true, "Exit");
         if (value.IsNil)
            return null;
         if (value.IsTrue)
            Runtime.State.ExitSignal = true;
         return null;
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => "exit";

      public bool IsEnd => true;

      public bool EvaluateFirst => true;
   }
}