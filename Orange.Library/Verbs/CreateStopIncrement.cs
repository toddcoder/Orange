using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class CreateStopIncrement : Verb
	{
		const string STR_LOCATION = "Create stop increment";
		public override Value Evaluate()
		{
			var increment = (Double)Runtime.State.Stack.Pop(true, STR_LOCATION).Number;
			var stop = (Double)Runtime.State.Stack.Pop(true, STR_LOCATION).Number;
			return new StopIncrement(stop, increment);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.StopIncrement;

	   public override bool LeftToRight => false;

	   public override string ToString() => ":";
	}
}