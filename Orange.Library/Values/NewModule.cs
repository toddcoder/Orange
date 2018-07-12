using Orange.Library.Managers;
using Orange.Library.Messages;

namespace Orange.Library.Values
{
   public class NewModule : Value, IMessageHandler
   {
      Block block;
      Region region;

      protected NewModule(Block block, Region region)
      {
         this.block = block;
         this.block.AutoRegister = false;
         this.region = region;
      }

      public NewModule(Block block)
         : this(block, new Region()) { }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; } = 0;

      public override ValueType Type => ValueType.Module;

      public override bool IsTrue => true;

      public override Value Clone() => new NewModule((Block)block.Clone(), region.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "evaluate", v => ((NewModule)v).Evaluate());
      }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         handled = false;

         if (region.ContainsMessage(messageName))
         {
            handled = true;
            return region[messageName];
         }

         return null;
      }

      public bool RespondsTo(string messageName) => region.ContainsMessage(messageName);

      public Value Evaluate() => block.Evaluate(region);

      public override string ToString() => region.ToString();
   }
}