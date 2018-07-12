using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class MBreakBlockElement : MBreakElement
	{
		Block textBlock;

		public MBreakBlockElement(Block textBlock, int count = -1)
			: base("", count) => this.textBlock = textBlock;

	   public override bool Evaluate(string input)
		{
			text = Expand(textBlock.Evaluate().Text);
			return base.Evaluate(input);
		}

		public override string ToString() => $"--({textBlock})";

	   public override Element Clone() => clone(new MBreakBlockElement((Block)textBlock.Clone(), count));

	   public override Element Alternate
		{
			get
			{
				text = Expand(textBlock.Evaluate().Text);
				return base.Alternate;
			}
			set => base.Alternate = value;
	   }
	}
}