using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class AppendToMessage : Verb
	{
		const string LOCATION = "Append to message";

		string messageName;

		public AppendToMessage(string messageName) => this.messageName = messageName;

	   public string MessageName => messageName;

	   public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var value = stack.Pop(true, LOCATION);
			MessageArguments messageArguments;
			if (stack.IsEmpty)
			{
				messageArguments = new MessageArguments();
				messageArguments.Add(messageName, value);
				return messageArguments;
			}
			if (stack.Peek(true, LOCATION).Type == Value.ValueType.MessageArguments)
			{
				messageArguments = (MessageArguments)stack.Pop(true, LOCATION);
				messageArguments.Add(messageName, value);
				return messageArguments;
			}
			messageArguments = new MessageArguments();
			messageArguments.Add(messageName, value);
			return messageArguments;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.ChangeSign;

	   public override string ToString() => messageName + ":";
	}
}