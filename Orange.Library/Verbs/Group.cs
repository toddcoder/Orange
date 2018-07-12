using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Group : Verb
	{
		const string LOCATION = "Group";

		public override Value Evaluate()
		{
		   var stack = State.Stack;
		   var executable = stack.Pop(true, LOCATION);
			var target = stack.Pop(true, LOCATION);
			var arguments = GuaranteedExecutable(executable);
			return MessagingState.SendMessage(target, "group", arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "group";
	}
}