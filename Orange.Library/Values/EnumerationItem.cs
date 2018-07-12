using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class EnumerationItem : Value
   {
      string enumerationName;
      string name;
      int value;

      public EnumerationItem(string enumerationName, string name, int value)
      {
         this.enumerationName = enumerationName;
         this.name = name;
         this.value = value;
      }

      public override int Compare(Value value) => this.value.CompareTo((int)value.Number);

      public override string Text
      {
         get => name;
         set { }
      }

      public override double Number
      {
         get => value;
         set { }
      }

      public override ValueType Type => ValueType.EnumerationItem;

      public override bool IsTrue => value != 0;

      public override Value Clone() => new EnumerationItem(enumerationName.Copy(), name.Copy(), value);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "name", v => ((EnumerationItem)v).Name());
         manager.RegisterMessage(this, "value", v => ((EnumerationItem)v).Value());
         manager.RegisterMessage(this, "enum", v => ((EnumerationItem)v).Enum());
      }

      public Value Name() => name;

      public Value Value() => value;

      public Value Enum() => enumerationName;

      public override Value AlternateValue(string message) => value;

      public override string ToString() => $"{enumerationName}.{name} => {value}";
   }
}