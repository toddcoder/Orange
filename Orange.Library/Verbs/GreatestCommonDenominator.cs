using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class GreatestCommonDenominator : Verb
	{
		const string LOCATION = "GCD";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var y = stack.Pop(true, LOCATION);
			var x = stack.Pop(true, LOCATION);
			return Runtime.SendMessage(x, "gcd", y);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Mod;

	   public override string ToString() => "gcd";
	}
}