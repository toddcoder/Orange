namespace Orange.Library.Verbs
{
   public class ShiftR : ShiftL
   {
      protected override string location() => "SHR";

      protected override string messageName() => "shr";

      public override string ToString() => ">>";
   }
}