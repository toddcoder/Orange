using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Macro : Value
   {
      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Macro;

      public override bool IsTrue => false;

      public override Value Clone() => new Macro();

      protected override void registerMessages(MessageManager manager) =>
         manager.RegisterMessage(this, "invoke", v => ((Macro)v).Invoke());

      public Value Invoke() => Invoke(Arguments);

      public Value Invoke(Arguments arguments, Region regionToUse = null) => null;
   }
}