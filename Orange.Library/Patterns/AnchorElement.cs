using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class AnchorElement : Element
	{
		public override bool Evaluate(string input)
		{
			State.Anchored = true;
			index = State.Position;
			length = 0;
			return false;
		}

		public override Element Clone() => clone(new AnchorElement());

	   public override string ToString() => "^";
	}
}