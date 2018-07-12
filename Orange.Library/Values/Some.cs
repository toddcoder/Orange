using Orange.Library.Managers;
using static Orange.Library.ParameterAssistant;

namespace Orange.Library.Values
{
   public class Some : Value
   {
      Value cargo;

      public Some(Value cargo) => this.cargo = cargo;

      public override int Compare(Value value) => value is Some s ? cargo.Compare(s.cargo) : -1;

      public override string Text
      {
         get => cargo.Text;
         set { }
      }

      public override double Number
      {
         get => cargo.Number;
         set { }
      }

      public override ValueType Type => ValueType.Some;

      public override bool IsTrue => true;

      public override Value Clone() => new Some(cargo.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((Some)v).Map());
         manager.RegisterMessage(this, "fmap", v => ((Some)v).FlatMap());
         manager.RegisterMessage(this, "value", v => ((Some)v).Value());
         manager.RegisterMessage(this, "defaultTo", v => ((Some)v).DefaultTo());
      }

      public Value Map()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.IteratorParameter();
            assistant.SetIteratorParameter(cargo);
            var result = block.Evaluate();

            var signal = Signal();
            if (signal != SignalType.None)
               return this;

            if (result.IsNil)
               return this;

            if (result is Some)
               return result;

            return new Some(result);
         }
      }

      public Value FlatMap()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.IteratorParameter();
            assistant.SetIteratorParameter(cargo);
            return block.Evaluate();
         }
      }

      public Value DefaultTo() => cargo;

      public Value Value() => cargo;

      public override string ToString() => $"{cargo}?";

      public (bool, string, Value) Match(Value right) => right is Some c ? (Case.Match(cargo, c.cargo, false, null), "", cargo) :
         (false, "", (Value)null);
   }
}