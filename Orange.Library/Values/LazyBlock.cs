using Orange.Library.Managers;
using Orange.Library.Messages;

namespace Orange.Library.Values
{
   public class LazyBlock : Value, IMessageHandler
   {
      Block block;
      Value value;

      public LazyBlock(Block block)
      {
         this.block = block;
         value = new Nil();
      }

      Value getValue()
      {
         if (value.IsNil)
            value = block.Evaluate();
         return value;
      }

      public override int Compare(Value aValue) => getValue().Compare(aValue);

      public override string Text
      {
         get => getValue().Text;
         set => getValue().Text = value;
      }

      public override double Number
      {
         get => getValue().Number;
         set => getValue().Number = value;
      }

      public override ValueType Type => ValueType.LazyBlock;

      public override bool IsTrue => getValue().IsTrue;

      public override Value Clone() => new LazyBlock(block);

      protected override void registerMessages(MessageManager manager) { }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         handled = true;
         return Runtime.SendMessage(getValue(), messageName, arguments);
      }

      public bool RespondsTo(string messageName) => MessageManager.MessagingState.RespondsTo(getValue(), messageName);

      public override string ToString() => value.IsNil ? $"lazy {block}" : getValue().ToString();
   }
}