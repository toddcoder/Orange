using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class OpenEndRange : NSLazyRange
   {
      public override Value Evaluate()
      {
         State.Stack.Push(new Variable("succ"));
         return base.Evaluate();
      }
   }
}