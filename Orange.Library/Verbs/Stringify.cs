using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Stringify : Verb
	{
		const string LOCATION = "Stringify";

		public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, LOCATION);
			return value.ToString();
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.ChangeSign;

	   public override string ToString() => "~";
	}
}