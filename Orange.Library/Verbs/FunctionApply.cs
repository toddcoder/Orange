using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Verbs
{
   public class FunctionApply : Verb
   {
      const string LOCATION = "Function Apply";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         if (x is Lambda lambda1 && y is Lambda lambda2)
            return new FunctionApplication(lambda1, lambda2);

         if (x is FunctionApplication application && y is Lambda lambda3)
         {
            application.Add(lambda3);
            return application;
         }

         return NilValue;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.SendMessage;

      public override string ToString() => ".";
   }
}