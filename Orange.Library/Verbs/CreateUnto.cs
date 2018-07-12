using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class CreateUnto : Verb
	{
		const string LOCATION = "Create unto";

		Unto unto;

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var stop = stack.Pop(true, LOCATION);
			var start = stack.Pop(true, LOCATION);
			if (unto == null)
				unto = new Unto();
			unto.SetStartAndStop(start, stop);
			return unto;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Equals;

	   public override string ToString() => "unto";
	}
}