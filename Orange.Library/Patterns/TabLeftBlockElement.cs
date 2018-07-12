using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class TabLeftBlockElement : TabLeftElement
	{
		Block at;

		public TabLeftBlockElement(Block at)
			: base(0) => this.at = at;

	   public TabLeftBlockElement()
			: this(null)
		{
		}

		public override bool Evaluate(string input)
		{
		   position = (int)(at.Evaluate()?.Number ?? 0);
			return base.Evaluate(input);
		}

		public override Element Clone() => clone(new TabLeftBlockElement((Block)at.Clone()));

	   public override string ToString() => $">({at})";
	}
}