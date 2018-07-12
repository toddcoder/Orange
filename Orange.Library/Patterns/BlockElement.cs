using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class BlockElement : StringElement
	{
		Block block;

		public BlockElement(Block block)
			: base("") => this.block = block;

	   public override bool EvaluateFirst(string input)
		{
			text = block.Evaluate().Text;
			return base.EvaluateFirst(input);
		}

		public override bool Evaluate(string input)
		{
			text = block.Evaluate().Text;
			return base.Evaluate(input);
		}

		public override Element Clone() => clone(new BlockElement((Block)block.Clone()));

	   public override string ToString() => "{" + block + "}";
	}
}