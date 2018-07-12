using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class SortDesc : Verb
	{
		const string LOCATION = "Sort Desc";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var controller = stack.Pop(true, LOCATION);
			var arrayValue = stack.Pop(true, LOCATION);
			var arguments = Arguments.FromValue(controller);
			return Runtime.SendMessage(arrayValue, "sortDesc", arguments);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "sortdesc";
	}
}