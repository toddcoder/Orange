using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class Alt : Verb
   {
      public override Value Evaluate()
      {
         var value = Runtime.State.Stack.Pop(true, "Alt");
         return Runtime.SendMessage(value, "alt");
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => "|>";
   }
}