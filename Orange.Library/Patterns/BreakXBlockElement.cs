using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class BreakXBlockElement : BreakXElement
	{
		Block block;

		public BreakXBlockElement(Block text, int matchIndex = 0)
			: base("", matchIndex) => block = text;

	   public BreakXBlockElement()
			: this(null)
		{
		}

		void setText() => text = block.Evaluate().Text;

	   public override bool Evaluate(string input)
		{
			setText();
			return base.Evaluate(input);
		}

		public override bool EvaluateFirst(string input)
		{
			setText();
			return base.EvaluateFirst(input);
		}

		public override Element Alternate
		{
			get
			{
				setText();
				return base.Alternate;
			}
			set => base.Alternate = value;
		}

		public override Element Clone()
		{
			setText();
			return base.Clone();
		}
	}
}