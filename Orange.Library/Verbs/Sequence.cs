using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Sequence : Verb
   {
      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "Sequence");
         return value is Object obj ? new ObjectSequence(obj) : MessagingState.SendMessage(value, "seq", new Arguments());
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "seq";
   }
}