using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class ArrayOperation : Verb
	{
		const string LOCATION = "Array operation ";

		string message;
		ExpressionManager.VerbPresidenceType presidence;

		public ArrayOperation(string message, ExpressionManager.VerbPresidenceType presidence)
		{
			this.message = message;
			this.presidence = presidence;
		}

		public override Value Evaluate()
		{
			var location = LOCATION + message;
			var y = Runtime.State.Stack.Pop(true, location);
			var x = Runtime.State.Stack.Pop(true, location);
			Runtime.Assert(x.IsArray, LOCATION, "Left-hand value must be an array");
			var array = (Array)x.SourceArray;
			return MessageManager.MessagingState.SendMessage(array, message, new Arguments(y));
		}

		public override ExpressionManager.VerbPresidenceType Presidence => presidence;
	}
}