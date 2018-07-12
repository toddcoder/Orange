using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Types.Collections;

namespace Orange.Library.Values
{
   public class Module : Value, IMessageHandler
   {
      Region region;

      public Module(Region region) => this.region = region;

      public Value this[string message]
      {
         get => region[message];
         set => region[message] = value;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Module;

      public override bool IsTrue => false;

      public override Value Clone() => new Module(region);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "use", v => ((Module)v).Use());
         manager.RegisterMessage(this, "has", v => ((Module)v).Has());
         manager.RegisterMessage(this, "vars", v => ((Module)v).Vars());
      }

      public Value Vars() => new Array(region.Locals.ToAutoHash(new Nil()));

      public Value Use()
      {
         var block = Arguments.Executable;
         if (block.CanExecute)
         {
            block.AutoRegister = false;
            Runtime.State.RegisterBlock(block, region);
            block.Evaluate();
            Runtime.State.UnregisterBlock();
         }
         else
            foreach (var item in region.Locals)
               RegionManager.Regions[item.Key] = item.Value;

         return this;
      }

      public Value Has() => region.Exists(Arguments[0].Text);

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         handled = true;
         var request = this[messageName];
         return request.Type == ValueType.Lambda ? ((Lambda)request).Evaluate(arguments) :
            new ModuleVariable(messageName, this);
      }

      public bool RespondsTo(string messageName) => region.Exists(messageName);
   }
}