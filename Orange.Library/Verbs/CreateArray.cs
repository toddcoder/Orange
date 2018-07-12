using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class CreateArray : Verb
	{
		const string STR_LOCATION = "Create array";

		public override Value Evaluate()
		{
			ValueStack stack = Runtime.State.Stack;
			bool resolve = Runtime.State.Resolve;
			if (stack.IsEmpty)
				return new Array();
			Value right = stack.Pop(resolve, STR_LOCATION);
			if (stack.IsEmpty)
			{
				if (right.IsArray)
					right = right.SourceArray;
				if (right.Type == Value.ValueType.Array)
					return right.Clone();
				if (right is InterpolatedString)
					right = right.Text;
				return new Array
				{
					right
				};
			}
			Value left = stack.Pop(resolve, STR_LOCATION);
			Array array;
			if (left.IsArray)
				left = left.SourceArray;
			if (left is InterpolatedString)
				left = left.Text;
			if (left.Type == Value.ValueType.Array)
			{
				var leftArray = (Array)left.Clone();
				if (leftArray.Packed)
					array = new Array
					{
						leftArray
					};
				else
					array = leftArray;
			}
			else
				array = new Array
				{
					left
				};
			if (right.IsArray)
				right = right.SourceArray;
			if (right is InterpolatedString)
				right = right.Text;
			if (right.Type == Value.ValueType.Array)
			{
				var rightArray = (Array)right.Clone();
				if (rightArray.Packed)
					array.Add(rightArray);
				else
					foreach (Array.IterItem item in rightArray)
						array.Add(item.Value);
			}
			else
				array.Add(right);
			return array;
		}

		public override string ToString()
		{
			return ",";
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.CreateArray;
			}
		}
	}
}