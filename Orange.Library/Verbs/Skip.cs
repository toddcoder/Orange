using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Skip : Verb
	{
		const string LOCATION = "Skip";

      public override Value Evaluate()
		{
			var stack = State.Stack;
			var value = stack.Pop(true, LOCATION);
			var target = stack.Pop(true, LOCATION);
			var arguments = FromValue(value, false);
         return MessagingState.SendMessage(target, "skip", arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "skip";
	}
}