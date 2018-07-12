using System.Text;
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class SendMessageToLastValue : Verb
	{
		const string LOCATION = "Send message to last value";

		string message;
		Arguments arguments;

		public SendMessageToLastValue(string message, Arguments arguments)
		{
			this.message = message;
			this.arguments = arguments;
		}

		public override Value Evaluate()
		{
			var value = MessageManager.MessagingState.LastValue;
			Runtime.RejectNull(value == null, LOCATION, "No previous object used");
			return MessageManager.MessagingState.Send(value, message, arguments);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.SendMessage;

	   public override string ToString()
		{
			var result = new StringBuilder();
			result.AppendFormat("?.{0}", message);
			if (arguments.Executable.CanExecute)
			{
				result.Append("{");
				result.Append(arguments.Executable);
				result.Append("}");
			}
			return result.ToString();
		}
	}
}