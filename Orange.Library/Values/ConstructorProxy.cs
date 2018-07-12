using Orange.Library.Managers;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class ConstructorProxy : Value
   {
      string dataName;
      string name;

      public ConstructorProxy(string dataName, string name)
      {
         this.dataName = dataName;
         this.name = name;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => $"{dataName}.{name}";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Constructor;

      public override bool IsTrue => true;

      public override Value Clone() => new ConstructorProxy(dataName, name);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((ConstructorProxy)v).Invoke());
      }

      public Value Invoke()
      {
         var value = RegionManager.Regions[dataName];
         if (value is Data data)
            return SendMessage(data, name, Arguments);

         Throw("Constructor proxy", $"{value} isn't a data");
         return null;
      }

      public string DataName => dataName;

      public string Name => name;
   }
}