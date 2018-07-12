namespace Orange.Library.Patterns
{
	public class TabRightElement : Element
	{
		protected int position;

		public TabRightElement(int position) => this.position = position;

	   public TabRightElement()
			: this(-1)
		{
		}

		public override bool Evaluate(string input)
		{
			var inputLength = input.Length;
			position = inputLength - position;
			if (position == -1 || position >= inputLength)
				return false;

			index = Runtime.State.Position;
			length = position - index;
			return length >= 0;
		}

		public override Element Clone() => clone(new TabRightElement(position));

	   public override string ToString() => $"<{position}";
	}
}