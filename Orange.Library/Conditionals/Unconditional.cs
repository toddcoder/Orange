using Orange.Library.Patterns;

namespace Orange.Library.Conditionals
{
	public class Unconditional : Conditional
	{
		public Unconditional()
			: base(null)
		{
		}

		public override bool Evaluate(Element element) => true;
	}
}