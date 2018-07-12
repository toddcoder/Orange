using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Keep : Verb
	{
		const string LOCATION = "Keep";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var value = stack.Pop(true, LOCATION);
			var target = stack.Pop(true, LOCATION);
			var arguments = Arguments.FromValue(value, false);
			return Runtime.SendMessage(target, "keep", arguments);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "keep";
	}
}