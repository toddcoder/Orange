using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Requires : Verb
	{
		const string LOCATION = "Require";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var y = stack.Pop(true, LOCATION);
			var x = stack.Pop(true, LOCATION);
			Assert(Case.Match(x, y, false, null), LOCATION, $"{x} doesn't match {y} requirement");
			return x;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "requires";
	}
}