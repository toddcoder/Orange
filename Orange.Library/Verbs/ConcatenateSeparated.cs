using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class ConcatenateSeparated : Verb
   {
      const string LOCATION = "Concatenate separated";

      public override Value Evaluate()
      {
         var y = Runtime.State.Stack.Pop(true, LOCATION);
         var x = Runtime.State.Stack.Pop(true, LOCATION);
         return x.Text + Runtime.State.FieldSeparator.Text + y.Text;
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Concatenate;
   }
}