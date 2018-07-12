using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class NotNil : Verb
   {
      public override Value Evaluate()
      {
         var value = Runtime.State.Stack.Pop(true, "Not nil");
         return value.IsTrue ? value : new Nil();
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => "??";
   }
}