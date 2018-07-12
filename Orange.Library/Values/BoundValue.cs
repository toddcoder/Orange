using Orange.Library.Managers;
using Orange.Library.Messages;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class BoundValue : Value, IMessageHandler
   {
      const string LOCATION = "Bound value";

      public static bool Unbind(Value value, out string name, out Value innerValue)
      {
         if (value is BoundValue boundValue)
         {
            name = boundValue.name;
            innerValue = boundValue.innerValue;
            return true;
         }

         name = "";
         innerValue = NilValue;
         return false;
      }

      string name;
      Value innerValue;

      public BoundValue(string name, Value innerValue)
      {
         this.name = name;
         this.innerValue = innerValue;
      }

      public string Name => name;

      public Value InnerValue => innerValue;

      public override int Compare(Value value) => innerValue.Compare(value);

      public override string Text
      {
         get => innerValue.Text;
         set { }
      }

      public override double Number
      {
         get => innerValue.Number;
         set { }
      }

      public override ValueType Type => ValueType.BoundValue;

      public override bool IsTrue => innerValue.IsTrue;

      public override Value Clone() => new BoundValue(name, innerValue.Clone());

      protected override void registerMessages(MessageManager manager) { }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         handled = false;
         Throw(LOCATION, "Value must be unbound first");
         return null;
      }

      public bool RespondsTo(string messageName)
      {
         Throw(LOCATION, "Value must be unbound first");
         return false;
      }

      public override string ToString() => $"{name} = {innerValue}";
   }
}