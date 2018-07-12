using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class From : Verb
	{
		const string LOCATION = "Create comprehension";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var source = stack.Pop(true, LOCATION, false);
			var target = stack.Pop(false, LOCATION);
			var arguments = Arguments.GuaranteedExecutable(source, false);
			return Runtime.SendMessage(target, "from", arguments);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "from";
	}
}