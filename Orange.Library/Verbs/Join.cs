using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Join : Verb
	{
		const string LOCATION = "Join";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var argument = stack.Pop(true, LOCATION);
			var target = stack.Pop(true, LOCATION);
			return MessageManager.MessagingState.RespondsTo(target, "join") ? Runtime.SendMessage(target, "join", argument) : target;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "join";
	}
}