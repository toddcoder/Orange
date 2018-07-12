using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class SendMessageToClass : Verb
	{
		const string LOCATION = "Send message to class";

		string message;
		Arguments arguments;

		public SendMessageToClass(string message, Arguments arguments)
		{
			this.message = message;
			this.arguments = arguments;
		}

		public string Message => message;

	   public Arguments Arguments => arguments;

	   public override Value Evaluate()
		{
			var cls = Regions["class"];
			Reject(cls.IsEmpty, LOCATION, $"{message} message called out of class");
			arguments.FromSelf = true;
	      return MessagingState.Send(cls, message, arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => $"@{message} {arguments}";
	}
}