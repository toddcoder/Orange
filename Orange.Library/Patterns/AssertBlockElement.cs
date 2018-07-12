using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class AssertBlockElement : AssertElement
	{
		Block block;

		public AssertBlockElement(Block block)
			: base("") => this.block = block;

	   public override bool Evaluate(string input)
		{
			text = block.Evaluate().Text;
			return base.Evaluate(input);
		}

		public override Element Clone() => new AssertBlockElement((Block)block.Clone());
	}
}