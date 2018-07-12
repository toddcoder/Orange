using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class ArrayConcatenate : Verb
	{
		const string LOCATION = "Array concatenate";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var y = stack.Pop(true, LOCATION);
			var x = stack.Pop(true, LOCATION);
			if (!x.IsArray)
			{
				var array = new Array
				{
					x
				};
				x = array;
			}
			return y.IsArray ? Array.Concatenate((Array)x.SourceArray, (Array)y.SourceArray) :Array.ConcatenateValue((Array)x.SourceArray, y);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.CreateArray;

	   public override string ToString() => "^";

	   public override bool LeftToRight => false;
	}
}