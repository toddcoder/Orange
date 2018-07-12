using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class CallRange : Verb
   {
      public override Value Evaluate()
      {
         var value = Runtime.State.Stack.Pop(true, "Call range");
         if (value.IsNumeric())
            return MessageManager.MessagingState.SendMessage(value, "range", new Arguments());
         var text = value.Text;
         return new Array(Runtime.State.RecordPattern.IsMatch(text) ? Runtime.State.RecordPattern.Split(text) :
            Runtime.State.FieldPattern.Split(text));
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Increment;

      public override string ToString() => "^";
   }
}