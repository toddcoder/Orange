using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Null : Value
   {
      public static Null NullValue => new Null();

      public override int Compare(Value value) => value is Null ? 0 : -1;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Null;

      public override bool IsTrue => false;

      public override Value Clone() => new Null();

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "append", v => ((Null)v).Append());
      }

      public Value Append() => new Buffer(Arguments[0].Text);

      public override string ToString() => "null";

      public override bool IsNull => true;

      public override int GetHashCode() => "null".GetHashCode();

      public override bool Equals(object obj) => obj is Null;
   }
}