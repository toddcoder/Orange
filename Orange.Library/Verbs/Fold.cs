using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Fold : Verb
   {
      Lambda lambda;
      string message;

      public Fold(Lambda lambda, bool leftToRight)
      {
         this.lambda = lambda;
         message = leftToRight ? "foldl" : "foldr";
      }

      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "Fold");
         var arguments = PipelineSource(lambda);
         return SendMessage(value, message, arguments);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.SendMessage;

      public override string ToString() => $"{message} {lambda}";
   }
}