namespace Orange.Library.Patterns
{
	public class RemElement : Element
	{
		public override bool Evaluate(string input)
		{
			index = Runtime.State.Position;
			length = input.Length - index;
			return true;
		}

		public override Element Clone() => new RemElement
		{
		   Next = cloneNext(),
		   Alternate = cloneAlternate(),
		   Replacement = cloneReplacement()
		};

	   public override string ToString() => "%";
	}
}