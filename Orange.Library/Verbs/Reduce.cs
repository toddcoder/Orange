using Orange.Library.Managers;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Reduce : Verb
   {
      const string LOCATION = "Reduce";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var operatorValue = stack.Pop(true, LOCATION);
         var value = stack.Pop(true, LOCATION);
         if (operatorValue is Lambda lambda)
         {
            var arguments = Arguments.FromValue(lambda);
            return SendMessage(value, "reduce", arguments);
         }

         if (!OperatorBlock(operatorValue).If(out var tuple))
            return value;

         (var block, var leftToRight) = tuple;
         var message = leftToRight ? "reduce" : "rreduce";
         return MessageManager.MessagingState.SendMessage(value, message, new Arguments(new Block(), block));
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "reduce";
   }
}