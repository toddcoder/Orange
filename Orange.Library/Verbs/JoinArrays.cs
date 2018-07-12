using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class JoinArrays : Verb
	{
		const string STR_LOCATION = "Join arrays";

		public override Value Evaluate()
		{
			Value y = Runtime.State.Stack.Pop(true, STR_LOCATION);
			Value x = Runtime.State.Stack.Pop(true, STR_LOCATION);
			Runtime.Assert(x.IsArray && y.IsArray, STR_LOCATION, "Only arrays can be arguments");
			var left = (Array)x.SourceArray;
			var right = (Array)y.SourceArray;
			return new JoinedArrays(left, right);
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.JoinArray;
			}
		}

		public override string ToString()
		{
			return "<>";
		}
	}
}