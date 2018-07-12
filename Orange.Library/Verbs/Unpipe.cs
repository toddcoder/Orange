using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Unpipe : Verb
   {
      const string LOCATION = "Unpipe";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var source = stack.Pop(true, LOCATION);
         var target = stack.Pop(true, LOCATION);
         return SendMessage(target, "invoke", source);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "<|";
   }
}