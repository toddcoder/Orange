using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class CreateRational : Verb
	{
		const string LOCATION = "Create rational";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var y = stack.Pop(true, LOCATION);
			var x = stack.Pop(true, LOCATION);
			return new Rational((int)x.Number, (int)y.Number);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Divide;

	   public override string ToString() => "|";
	}
}