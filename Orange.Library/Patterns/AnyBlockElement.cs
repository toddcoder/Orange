using System.Text;
using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class AnyBlockElement : AnyElement
	{
		Block block;

		public AnyBlockElement(Block block, int count)
			: base("", count) => this.block = block;

	   public override bool Evaluate(string input)
		{
			var printBlock = new PrintBlock(block);
			text = Expand(printBlock.String.Text);
			return base.Evaluate(input);
		}

		public override bool EvaluateFirst(string input)
		{
			text = Expand(block.Evaluate().Text);
			return base.EvaluateFirst(input);
		}

		public override Element Clone() => clone(new AnyBlockElement((Block)block.Clone(), count));

	   public override string ToString()
		{
			var result = new StringBuilder();
			if (Not)
				result.Append("!");
			if (count > 1)
				result.Append(count);
			result.Append("(");
			result.Append(Expand(block.Evaluate().Text));
			result.Append(")");
			return result.ToString();
		}
	}
}