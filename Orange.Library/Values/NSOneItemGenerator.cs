using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class NSOneItemGenerator : NSGenerator
   {
      bool discharged;
      Value value;

      public NSOneItemGenerator(Value value)
         : base(null)
      {
         discharged = false;
         this.value = value;
      }

      public override Value Clone() => new NSOneItemGenerator(value.Clone());

      public override void Reset() => discharged = false;

      public override Value Next()
      {
         if (discharged)
            return NilValue;
         discharged = true;
         return value;
      }
   }
}