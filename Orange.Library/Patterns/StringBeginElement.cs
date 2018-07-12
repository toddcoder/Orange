namespace Orange.Library.Patterns
{
	public class StringBeginElement : Element
	{
		public override bool Evaluate(string input)
		{
			index = Runtime.State.Position;
			if (Not && index != 0 || !Not && index == 0)
			{
				length = 0;
				return true;
			}
			return false;
		}

		public override Element Clone() => new StringBeginElement
		{
		   Next = cloneNext(),
		   Alternate = cloneAlternate(),
		   Replacement = cloneReplacement()
		};

	   public override string ToString() => (Not ? "!" : "") + "<";
	}
}