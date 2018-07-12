using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Partition : Value
   {
      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Partition;

      public override bool IsTrue => false;

      public override Value Clone() => null;

      protected override void registerMessages(MessageManager manager) { }

      public override Value AlternateValue(string message) => null;
   }
}