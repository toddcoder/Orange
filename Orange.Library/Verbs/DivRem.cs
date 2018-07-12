using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class DivRem : Verb
   {
      const string LOCATION = "Div rem";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var divisor = stack.Pop(true, LOCATION);
         var dividend = stack.Pop(true, LOCATION);
         return SendMessage(dividend, "divrem", divisor);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Divide;
   }
}