using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Take : Verb
	{
		const string LOCATION = "Take";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var value = stack.Pop(true, LOCATION);
			var target = stack.Pop(true, LOCATION);
			var arguments = FromValue(value, false);
		   return MessagingState.SendMessage(target, "take", arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "take";
	}
}