using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class ValueOperator : Verb
   {
      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "Value");
         return SendMessage(value, "value");
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.SendMessage;

      public override string ToString() => "!";
   }
}