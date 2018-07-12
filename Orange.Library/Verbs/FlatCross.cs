using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class FlatCross : Cross
   {
      protected override Value modifyInnerArray(Array array) => array.Flatten();

      public override string ToString() => "><";
   }
}