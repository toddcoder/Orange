using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Plus : Verb
	{
		const string LOCATION = "Plus";

		public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, LOCATION);
			if (value.IsArray)
			{
				var array = (Array)value.SourceArray;
				return array.Flatten();
			}
			return value.Number;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.ChangeSign;

	   public override string ToString() => "+";
	}
}