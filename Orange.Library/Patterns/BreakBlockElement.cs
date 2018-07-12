using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class BreakBlockElement : BreakElement
	{
		Block textBlock;

		public BreakBlockElement(Block textBlock)
			: base("") => this.textBlock = textBlock;

	   public override bool Evaluate(string input)
		{
			text = Runtime.Expand(textBlock.Evaluate().Text);
			return base.Evaluate(input);
		}

		public override bool EvaluateFirst(string input)
		{
			text = Runtime.Expand(textBlock.Evaluate().Text);
			return base.EvaluateFirst(input);
		}

		public override string ToString() => $"-({textBlock})";

	   public override Element Clone() => clone(new BreakBlockElement((Block)textBlock.Clone()));
	}
}