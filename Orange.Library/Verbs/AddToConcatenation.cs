using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class AddToConcatenation : Verb
	{
		const string LOCATION = "Add to concatenation";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var value = stack.Pop(true, LOCATION);
			Values.Concatenation concatenation;
			if (stack.IsEmpty)
			{
				concatenation = new Values.Concatenation();
				concatenation.Add(value);
			}
			else
			{
				var possibleConcatenation = stack.Pop(true, LOCATION);
				if (possibleConcatenation.Type == Value.ValueType.Concatenation)
				{
					concatenation = (Values.Concatenation)possibleConcatenation;
					concatenation.Add(value);
				}
				else
				{
					concatenation = new Values.Concatenation();
					concatenation.Add(possibleConcatenation);
					concatenation.Add(value);
				}
			}
			return concatenation;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.CreateArray;

	   public override string ToString() => "&&";
	}
}