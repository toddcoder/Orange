using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Iterator : Verb
	{
		const string LOCATION = "Iterator";

		public override Value Evaluate()
		{
		   var stack = State.Stack;
		   var yValue = stack.Pop(true, LOCATION);
			var xValue = stack.Pop(true, LOCATION);
			var iterator = xValue as IIterator;
			if (iterator != null)
			{
				iterator.Increment = yValue;
				return xValue;
			}
			if (xValue.Type == Value.ValueType.Number && yValue.Type == Value.ValueType.Number)
			{
				var dStart = xValue.Number;
				var dStop = yValue.Number;
				var iStart = (int)dStart;
				var iStop = (int)dStop;
				return (dStart == iStart && dStop == iStop) ? (Value)new IntIterator(iStart, iStop) : new DoubleIterator(dStart, dStop);
			}

			return new StringIterator(xValue.Text, yValue.Text);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

	   public override string ToString() => "|";
	}
}