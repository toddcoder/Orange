using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Map : Verb
	{
		const string LOCATION = "Map";

		public override Value Evaluate()
		{
		   var stack = State.Stack;
		   var source = stack.Pop(true, LOCATION, false);
			var target = stack.Pop(false, LOCATION);
			var arguments = GuaranteedExecutable(source);
		   return MessagingState.SendMessage(target, "map", arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "map";
	}
}