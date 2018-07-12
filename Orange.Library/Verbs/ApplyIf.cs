using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class ApplyIf : Verb
	{
		const string LOCATION = "Apply if";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var target = stack.Pop(true, LOCATION, false);
			var subject = stack.Pop(false, LOCATION);
			var arguments = new Arguments();
			if (subject.IsVariable)
			{
				var variable = (Variable)subject;
				arguments.ApplyVariable = variable;
				arguments.ApplyValue = variable.Value;
			}
			else
			{
				arguments.ApplyVariable = null;
				arguments.ApplyValue = subject;
			}
			return MessageManager.MessagingState.SendMessage(target, "applyIf", arguments);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "|?";
	}
}