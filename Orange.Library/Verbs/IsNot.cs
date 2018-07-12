using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class IsNot : Is
   {
      public override Value Evaluate() => !base.Evaluate().IsTrue;

      public override string ToString() => "is not";

      public override string Location => "Is not";
   }
}