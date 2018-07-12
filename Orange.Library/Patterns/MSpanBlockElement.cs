using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class MSpanBlockElement : MSpanElement
	{
		Block textBlock;

		public MSpanBlockElement(Block textBlock, int count = -1)
			: base("", count) => this.textBlock = textBlock;

	   public override bool Evaluate(string input)
		{
			text = Expand(textBlock.Evaluate().Text);
			return base.Evaluate(input);
		}

		public override Element Alternate
		{
			get
			{
				text = Expand(textBlock.Evaluate().Text);
				return base.Alternate;
			}
			set => base.Alternate = value;
		}

		public override string ToString() => $"++({textBlock})";

	   public override Element Clone() => clone(new MSpanBlockElement(textBlock, count));
	}
}