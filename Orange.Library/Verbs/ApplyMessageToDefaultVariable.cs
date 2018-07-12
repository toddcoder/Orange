using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class ApplyMessageToDefaultVariable : Verb
	{
		Message message;

		public ApplyMessageToDefaultVariable(Message message) => this.message = message;

	   public override Value Evaluate()
		{
			var defaultVariableName = Runtime.State.DefaultParameterNames.ValueVariable;
			var variable = new Variable(defaultVariableName);
			var block = new Block
			{
				new Push(variable),
				new SendMessage(message.MessageName, message.MessageArguments)
			};
			return block;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.SendMessage;

	   public override string ToString() => message.ToString();
	}
}