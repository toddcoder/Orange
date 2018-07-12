using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class ConvertToArray : Verb
	{
		public override Value Evaluate()
		{
			var value = Runtime.State.Stack.Pop(true, "Convert to array");
			if (value.Type == Value.ValueType.Sequence)
				return Runtime.SendMessage(value, "arr");
			return value.IsArray ? Runtime.SendMessage(value, "flat") : new Array
			{
				value
			};
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "@";
	}
}