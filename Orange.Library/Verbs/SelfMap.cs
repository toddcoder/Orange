using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class SelfMap : Verb
   {
      const string LOCATION = "Self Map";
      const string MESSAGE_NAME = "smap";

      public override Value Evaluate()
      {
         var source = Runtime.State.Stack.Pop(true, LOCATION, false);
         var target = Runtime.State.Stack.Pop(false, LOCATION);
         if (!target.IsIndexer)
            target = target.Resolve();
         var arguments = Arguments.GuaranteedExecutable(source);
         return MessageManager.MessagingState.SendMessage(target, MESSAGE_NAME, arguments);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => "-->";
   }
}