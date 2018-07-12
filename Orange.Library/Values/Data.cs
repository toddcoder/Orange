using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Enumerables;

namespace Orange.Library.Values
{
   public class Data : Value, IMessageHandler
   {
      string name;
      Hash<string, int> constructors;

      public Data(string name, Hash<string, int> constructors)
      {
         this.name = name;
         this.constructors = constructors;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return $"type {name} = {constructors.KeyArray().Listify(" | ")}"; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Data;

      public override bool IsTrue => true;

      public override Value Clone() => new Data(name, constructors);

      protected override void registerMessages(MessageManager manager) { }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         handled = false;
         if (constructors.ContainsKey(messageName))
         {
            var count = constructors[messageName];
            var slice = arguments.Values.Slice(0, count);
            handled = true;
            return new Constructor(name, messageName, slice);
         }

         return Null.NullValue;
      }

      public bool RespondsTo(string messageName) => constructors.ContainsKey(messageName);

      public override string ToString() => Text;

      public string Name => name;

      public Hash<string, int> Constructors => constructors;
   }
}