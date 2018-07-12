using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class SkipSignal : Verb, IEnd
	{
		public override Value Evaluate()
		{
			var stack = State.Stack;
			if (stack.IsEmpty)
			{
				State.SkipSignal = true;
				return null;
			}
			var value = stack.Pop(true, "Skip");
			if (value.IsNil)
				return null;
			if (value.IsTrue)
				State.SkipSignal = true;
			return null;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "skip";

	   public bool IsEnd => true;

	   public bool EvaluateFirst => true;
	}
}