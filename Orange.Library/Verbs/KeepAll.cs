using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class KeepAll : Verb
   {
      const string LOCATION = "Keep all";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         var arguments = Arguments.GuaranteedExecutable(y);
         return Runtime.SendMessage(x, "keep-all", arguments);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => "%-";
   }
}