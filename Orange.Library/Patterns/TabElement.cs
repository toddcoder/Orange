using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class TabElement : Element
	{
		Block at;
		bool right;

		public TabElement(Block at, bool right)
		{
			this.at = at;
			this.right = right;
		}

		public override bool Evaluate(string input)
		{
		   var position = (int)(at.Evaluate()?.Number ?? -1);
			var inputLength = input.Length;
			if (right)
				position = inputLength - position;
			if (position == -1 || position >= inputLength)
				return false;

			index = Runtime.State.Position;
			length = position - index;
			return length >= 0;
		}

		public override Element Clone() => new TabElement((Block)at.Clone(), right)
		{
		   Next = cloneNext(),
		   Alternate = cloneAlternate(),
		   Replacement = cloneReplacement()
		};

	   public override string ToString() => $"{(right ? "<" : ">")}({at})";
	}
}