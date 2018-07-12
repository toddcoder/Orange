using Orange.Library.Values;
using static System.Math;

namespace Orange.Library.Verbs
{
   public class NSNegBy : NSBy
   {
      protected override Value increment(ValueStack stack) => -Abs(base.increment(stack).Int);

      public override string ToString() => ".-";
   }
}