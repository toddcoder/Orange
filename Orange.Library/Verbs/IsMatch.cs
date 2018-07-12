using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class IsMatch : Verb
   {
      const string LOCATION = "Is match";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         return SendMessage(x.Text, "isMatch", y);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Equals;

      public override string ToString() => "=~";
   }
}