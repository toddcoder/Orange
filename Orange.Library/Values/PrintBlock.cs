using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class PrintBlock : Value, IStringify
   {
      Block block;

      public PrintBlock(Block block) => this.block = block;

      public PrintBlock()
         : this(new Block()) { }

      public override int Compare(Value value) => string.CompareOrdinal(getText(), value.Text);

      public override string Text
      {
         get => getText();
         set { }
      }

      public override double Number
      {
         get => getText().ToDouble();
         set { }
      }

      public override ValueType Type => ValueType.PrintBlock;

      public override bool IsTrue => getText().IsNotEmpty();

      public override Value Clone() => new PrintBlock((Block)block.Clone());

      protected override void registerMessages(MessageManager manager) { }

      public override Value AlternateValue(string message) => getText();

      string getText()
      {
         var value = block.Evaluate();
         return value.IsArray ? ((Array)value.SourceArray).Values.Select(v => v.Text).Listify("") : value.Text;
      }

      public override string ToString() => $"${{{block}}}";

      public Value String => getText();

      public override Value AssignmentValue() => getText();

      public override Value ArgumentValue() => getText();
   }
}