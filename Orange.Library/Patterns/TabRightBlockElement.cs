using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class TabRightBlockElement : TabRightElement
	{
		Block at;

		public TabRightBlockElement(Block at)
			: base(0) => this.at = at;

	   public TabRightBlockElement()
			: this(null)
		{
		}

		public override bool Evaluate(string input)
		{
		   position = (int)(at.Evaluate()?.Number ?? 0);
			return base.Evaluate(input);
		}

		public override Element Clone() => clone(new TabRightBlockElement((Block)at.Clone()));

	   public override string ToString() => $"<({at})";
	}
}