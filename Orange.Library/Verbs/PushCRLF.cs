using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class PushCRLF : Verb
   {
      const string LOCATION = "Concatenate CRLF";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         return x.Text + "\r\n" + y.Text;
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Concatenate;

      public override string ToString() => "'`r`n'";
   }
}