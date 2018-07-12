using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Dereference : Verb
	{
		const string LOCATION = "Dereference";

		public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, LOCATION);
			return new Variable(value.Text);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Increment;

	   public override string ToString() => "&";

	   public override int OperandCount => 1;
	}
}