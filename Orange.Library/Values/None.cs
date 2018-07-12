using Orange.Library.Managers;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Null;

namespace Orange.Library.Values
{
   public class None : Value
   {
      public static None NoneValue => new None();

      public override int Compare(Value value) => value is None ? 0 : -1;

      public override string Text
      {
         get;
         set;
      } = "";

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.None;

      public override bool IsTrue => false;

      public override Value Clone() => new None();

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((None)v).Map());
         manager.RegisterMessage(this, "fmap", v => ((None)v).FlatMap());
         manager.RegisterMessage(this, "value", v => Value());
         manager.RegisterMessage(this, "defaultTo", v => ((None)v).DefaultTo());
      }

      public Value Map() => this;

      public Value FlatMap() => this;

      public static Value Value()
      {
         Assert(false, "None", "None has no value");
         return null;
      }

      public Value DefaultTo()
      {
         var value = Arguments[0];
         if (!value.IsEmpty)
            return value;

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return NullValue;
            return block.Evaluate();
         }
      }

      public override string ToString() => "none";
   }
}