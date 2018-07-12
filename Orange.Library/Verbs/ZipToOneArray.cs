using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class ZipToOneArray : Verb
	{
		const string LOCATION = "Zip to 1 array";

		public override Value Evaluate()
		{
			var y = Runtime.State.Stack.Pop(true, LOCATION);
			Runtime.Assert(y.IsArray, LOCATION, "Array needed for zip");
			var x = Runtime.State.Stack.Pop(true, LOCATION);
			Runtime.Assert(x.IsArray, LOCATION, "Array needed for zip");
			var left = (Array)x.SourceArray;
			var right = (Array)y.SourceArray;
			return MessageManager.MessagingState.SendMessage(left, "zipTo1", new Arguments(right));
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "-%%";
	}
}