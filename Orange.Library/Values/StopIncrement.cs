using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class StopIncrement : Value
   {
      Double stop;
      Double increment;

      public StopIncrement(Double stop, Double increment)
      {
         this.stop = stop;
         this.increment = increment;
      }

      public Double Stop => stop;

      public Double Increment => increment;

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.StopIncrement;

      public override bool IsTrue => false;

      public override Value Clone() => null;

      protected override void registerMessages(MessageManager manager) { }

      public override string ToString() => "|";
   }
}