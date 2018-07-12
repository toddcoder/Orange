using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Else : Verb
	{
		const string LOCATION = "Unless";

		public override Value Evaluate()
		{
		   var stack = State.Stack;
		   var y = stack.Pop(true, LOCATION, false);
			var x = stack.Pop(true, LOCATION);
			var arguments = PipelineSource(y);
			return MessagingState.SendMessage(x, "else", arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "else";
	}
}