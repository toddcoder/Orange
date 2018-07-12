using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class MessageVerb : Verb
   {
      const string LOCATION = "Message verb";

      string message;
      ValueStack stack;

      public MessageVerb(string message) => this.message = message;

      public override Value Evaluate()
      {
         stack = Runtime.State.Stack;
         var argumentValue = stack.Pop(true, LOCATION);
         var target = stack.Pop(true, LOCATION);
         var arguments = new Arguments(argumentValue);
         return Runtime.SendMessage(target, message, arguments);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => message + ":";
   }
}