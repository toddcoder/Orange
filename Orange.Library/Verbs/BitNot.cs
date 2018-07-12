using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class BitNot : Verb
	{
		const string LOCATION = "Bit not";

		public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, LOCATION);
			return ~(int)value.Number;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Not;

	   public override string ToString() => "~";
	}
}