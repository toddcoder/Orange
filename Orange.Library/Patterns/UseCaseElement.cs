namespace Orange.Library.Patterns
{
	public class UseCaseElement : Element
	{
		public override bool Evaluate(string input)
		{
			Runtime.State.IgnoreCase = false;
			index = Runtime.State.Position;
			length = 0;
			return true;
		}

		public override Element Clone() => clone(new UseCaseElement());

	   public override string ToString() => "&";
	}
}