using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Objects;

namespace Orange.Library.Verbs
{
	public class CaseAnd : Verb
	{
		const string LOCATION = "Case and";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var y = stack.Pop(true, LOCATION);
			var x = stack.Pop(true, LOCATION);
			Object obj1;
			Object obj2;
			if (x.IsA(out obj1) && y.IsA(out obj2))
			{
				return Case.MatchObjects(obj1, obj2, false);
			}
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Push;
			}
		}
	}
}