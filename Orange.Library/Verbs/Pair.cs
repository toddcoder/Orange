using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Pair : Verb
	{
		const string LOCATION = "Pair";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var right = stack.Pop(true, LOCATION);
			var left = stack.Pop(true, LOCATION);
			return new Values.Pair(left, right);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.KeyedValue;

	   public override string ToString() => @"\";
	}
}