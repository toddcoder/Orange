using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class ApplyInvoke : Verb
   {
      Arguments arguments;

      public ApplyInvoke(Arguments arguments) => this.arguments = arguments;

      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "Apply Invoke");
         return SendMessage(value.Self, "invoke", arguments);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => $":({arguments})";
   }
}