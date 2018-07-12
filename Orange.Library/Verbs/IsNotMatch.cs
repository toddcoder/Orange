using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class IsNotMatch : Verb
   {
      const string LOCATION = "Is not match";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         return SendMessage(x, "isNotMatch", y);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.NotEqual;

      public override string ToString() => "!~";
   }
}