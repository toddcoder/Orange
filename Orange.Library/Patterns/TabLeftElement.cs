namespace Orange.Library.Patterns
{
	public class TabLeftElement : Element
	{
		protected int position;

		public TabLeftElement(int position) => this.position = position;

	   public override bool Evaluate(string input)
		{
			var inputLength = input.Length;
			if (position == -1 || position >= inputLength)
				return false;

			index = Runtime.State.Position;
			length = position - index;
			return length >= 0;
		}

		public override Element Clone() => clone(new TabLeftElement(position));

	   public override string ToString() => $">{position}";
	}
}