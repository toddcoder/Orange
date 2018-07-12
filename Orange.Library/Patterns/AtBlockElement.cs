using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class AtBlockElement : AtElement
	{
		Block at;

		public AtBlockElement(Block at)
			: base(0) => this.at = at;

	   public override bool Evaluate(string input)
		{
		   position = (int)(at.Evaluate()?.Number ?? 0);
			return base.Evaluate(input);
		}

		public override Element Clone() => new AtBlockElement((Block)at.Clone());

	   public override string ToString() => $"@({at})";
	}
}