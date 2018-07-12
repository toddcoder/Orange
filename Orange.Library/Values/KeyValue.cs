using Orange.Library.Managers;
using static System.StringComparison;

namespace Orange.Library.Values
{
   public class KeyValue : Value
   {
      Value value;
      string key;
      int index;

      public KeyValue(Value value, string key, int index)
      {
         this.value = value;
         this.key = key;
         this.index = index;
      }

      public override int Compare(Value value) => value is KeyValue kv ? this.value.Compare(kv.value) +
         string.Compare(key, kv.key, Ordinal) + index.CompareTo(kv.index) : -1;

      public override string Text
      {
         get => value.Text;
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.KeyValue;

      public override bool IsTrue => false;

      public override Value Clone() => new KeyValue(value.Clone(), key, index);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "value", v => ((KeyValue)v).Value());
         manager.RegisterMessage(this, "key", v => ((KeyValue)v).Key());
         manager.RegisterMessage(this, "index", v => ((KeyValue)v).Index());
      }

      public Value Value() => value;

      public Value Key() => key;

      public Value Index() => index;

      public override string ToString() => $"{key} => {value} @ {index}";
   }
}