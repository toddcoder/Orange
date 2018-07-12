using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Split : Verb
   {
      const string LOCATION = "Split";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var pattern = stack.Pop(true, LOCATION);
         var value = stack.Pop(true, LOCATION);
         var arguments = GuaranteedExecutable(pattern, false);
         return SendMessage(value, "split", arguments);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "split";

   }
}