namespace Orange.Library.Patterns
{
	public class AbortElement : Element
	{
		public override bool Evaluate(string input) => false;

	   public override bool Aborted => true;

	   public override Element Clone() => clone(new AbortElement());

	   public override string ToString() => "-";
	}
}