using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class SpanBlockElement : SpanElement
	{
		Block textBlock;

		public SpanBlockElement(Block textBlock)
			: base("") => this.textBlock = textBlock;

	   public override bool Evaluate(string input)
		{
			var printBlock = new PrintBlock(textBlock);
			text = Runtime.Expand(printBlock.String.Text);
			return base.Evaluate(input);
		}

		public override bool EvaluateFirst(string input)
		{
			text = Runtime.Expand(textBlock.Evaluate().Text);
			return base.EvaluateFirst(input);
		}

		public override Element Clone() => clone(new SpanBlockElement((Block)textBlock.Clone()));

	   public override string ToString() => $"+({textBlock})";
	}
}