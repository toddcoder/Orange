using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class During : Verb
	{
		const string LOCATION = "During";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var limit = stack.Pop(true, LOCATION);
			var seed = stack.Pop(true, LOCATION);
			return new ArrayStream(seed, ParameterBlock.FromExecutable(limit));
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "during";
	}
}