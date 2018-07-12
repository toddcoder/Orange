using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class ApplyToMessage : Verb
	{
		Message message;

		public ApplyToMessage(Message message) => this.message = message;

	   public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, "Apply to message");
			return message.Invoke(value.ArgumentValue());
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "?" + message;
	}
}