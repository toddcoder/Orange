using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Order : Verb
	{
		const string LOCATION = "Order";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var controller = stack.Pop(true, LOCATION);
			var arrayValue = stack.Pop(true, LOCATION);
			/*			if (arrayValue.IsArray)
						{*/
			var arguments = Arguments.FromValue(controller);
			return arguments != null ? Runtime.SendMessage(arrayValue, "orderBy", arguments) : arrayValue;
			/*			}
						return arrayValue;*/
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "orderby";
	}
}