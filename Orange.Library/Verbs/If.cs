using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class If : Verb
	{
		const string LOCATION = "If";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var y = stack.Pop(true, LOCATION, false);
			var x = stack.Pop(true, LOCATION);
			var arguments = PipelineSource(y);
		   return MessagingState.SendMessage(x, "if", arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "if";
	}
}