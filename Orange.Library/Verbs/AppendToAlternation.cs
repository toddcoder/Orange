using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class AppendToAlternation : Verb
	{
		const string LOCATION = "Append to alternation";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var right = stack.Pop(true, LOCATION);
			var left = stack.Pop(false, LOCATION);
			Alternation alternation;
			if (left.Type == Value.ValueType.Alternation)
			{
				if (left is Variable)
					left = left.Resolve();
				alternation = left as Alternation;
				alternation.Add(right);
				return left;
			}
			alternation = new Alternation();
			alternation.Add(left);
			alternation.Add(right);
			return alternation;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.CreateAlternator;

	   public override string ToString() => "but";
	}
}