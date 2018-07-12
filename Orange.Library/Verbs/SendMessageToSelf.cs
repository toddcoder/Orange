using System.Text;
using Orange.Library.Managers;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class SendMessageToSelf : Verb
	{
		const string LOCATION = "Send message to self";

		string message;
		Arguments arguments;

		public SendMessageToSelf(string message, Arguments arguments)
		{
			this.message = message;
			this.arguments = arguments;
		}

		public SendMessageToSelf()
			: this("", null)
		{
		}

		public override Value Evaluate()
		{
			var self = RegionManager.Regions["self"];
			RejectNull(self, LOCATION, "Self not set");
			arguments.FromSelf = true;
			return MessagingState.Send(self, message, arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.SendMessage;

	   public override string ToString()
		{
			var result = new StringBuilder();
			result.Append($".{message}");
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