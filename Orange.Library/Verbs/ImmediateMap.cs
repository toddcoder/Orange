using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class ImmediateMap : Map
   {
      public override Value Evaluate()
      {
         var result = base.Evaluate();
         return SendMessage(result, "array");
      }
   }
}