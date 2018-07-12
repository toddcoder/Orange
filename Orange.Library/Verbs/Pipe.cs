using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Pipe : Verb
   {
      const string LOCATION = "Pipe";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var target = stack.Pop(true, LOCATION);
         var source = stack.Pop(true, LOCATION);
         return SendMessage(target, "invoke", source);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "|>";
   }
}