using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class At : Verb
	{
		const string LOCATION = "At";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var index = stack.Pop(true, LOCATION);
			var target = stack.Pop(true, LOCATION);
			return SendMessage(target, "at", index);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.KeyedValue;

	   public override string ToString() => "@";
	}
}