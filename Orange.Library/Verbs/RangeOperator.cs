using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class RangeOperator : Verb
   {
      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "range");
         return SendMessage(value, "range");
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.PreIncrement;

      public override string ToString() => "^";
   }
}