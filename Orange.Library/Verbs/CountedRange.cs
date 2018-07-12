using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Maybe;
using Standard.Types.Strings;

namespace Orange.Library.Verbs
{
	public class CountedRange : Verb, IWrapping
	{
		const string LOCATION = "Counted range";

		int? length;

		public CountedRange()
		{
			length = null;
			IsSlice = false;
		}

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var right = stack.Pop(true, LOCATION);
			var left = stack.Pop(true, LOCATION);
			if (left.Type == Value.ValueType.Range)
			{
				((IRange)left).Increment = (int)right.Number;
				return left;
			}
			if (left.Type == Value.ValueType.Number && right.Type == Value.ValueType.Number)
			{
				var start = (int)left.Number;
				if (length.HasValue && start < 0)
					start = Runtime.WrapIndex(start, length.Value, true);
				var stop = start + (int)right.Number - 1;
				if (right.Number == 0)
					stop = start;
				return new IntRange(start, stop, new None<int>());
			}
			if (left.Type == Value.ValueType.String && right.Type == Value.ValueType.Number)
			{
				var start = left.Text;
				Runtime.Reject(start.IsEmpty(), LOCATION, "String must have content");
				start = start.Substring(0, 1);
				var stop = ((char)(start[0] + (int)right.Number - 1)).ToString();
				return new StringRange(start, stop);
			}
			return right;
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Range;
			}
		}

		public override bool LeftToRight
		{
			get
			{
				return false;
			}
		}

		public override string ToString()
		{
			return "plus";
		}

		public void SetLength(int length)
		{
			this.length = length;
		}

		public bool IsSlice
		{
			get;
			set;
		}
	}
}