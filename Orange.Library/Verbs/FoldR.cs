using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class FoldR : Verb
   {
      const string LOCATION = "foldr";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var operatorValue = stack.Pop(true, LOCATION);
         var value = stack.Pop(true, LOCATION).Self;
         if (operatorValue is Lambda lambda)
            return MessagingState.SendMessage(value, "foldr", FromValue(lambda));

         Throw(LOCATION, "Expected lambda");
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "foldr";
   }
}