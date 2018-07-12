namespace Orange.Library.Patterns
{
	public class NegateElement : Element
	{
		public override bool Evaluate(string input) => false;

	   public override Element Clone() => clone(new NegateElement());

	   public override string ToString() => "!";
	}
}