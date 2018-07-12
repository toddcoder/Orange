using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Fields : Verb
	{
		const string LOCATION = "Fields";
		const string MESSAGE_NAME = "fields";

		public override Value Evaluate()
		{
			var code = Runtime.State.Stack.Pop(true, LOCATION, false);
			var arrayValue = Runtime.State.Stack.Pop(true, LOCATION);
			if (arrayValue.IsArray)
			{
				var array = (Array)arrayValue.SourceArray;
				var closure = code as Lambda;
				if (closure != null)
				{
					var arguments = new Arguments(new NullBlock(), closure.Block, closure.Parameters);
					return MessageManager.MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
				}
				if (code is IStringify)
				{
					var block = new Block
					{
						new Push(code)
					};
					var arguments = new Arguments(new NullBlock(), block, new NullParameters());
					return MessageManager.MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
				}
				var format = code as Values.Format;
				if (format != null)
				{
					var block = new Block
					{
						new Push(format.Stringify.String)
					};
					var arguments = new Arguments(new NullBlock(), block, format.Parameters);
					return MessageManager.MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
				}
			}
			return arrayValue;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "-|";
	}
}