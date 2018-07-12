using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Sort : Verb
	{
		const string LOCATION = "Sort";

		public override Value Evaluate()
		{
			var stack = Runtime.State.Stack;
			var controller = stack.Pop(true, LOCATION);
			var arrayValue = stack.Pop(true, LOCATION);
/*			if (arrayValue.IsArray)
			{
				if (controller.IsArray)
				{
					var parameterBlocks = ((Array)controller.SourceArray).Select(i => ParameterBlock.FromExecutable(i.Value))
						.Where(pb => pb != null).ToArray();
					return parameterBlocks.Length > 0 ? ((Array)arrayValue.SourceArray).SortBy(parameterBlocks.ToArray()) : arrayValue;
				}
				var arguments = Arguments.FromExecutable(controller);
				return arguments != null ? MessageManager.State.SendMessage(arrayValue, "sort", arguments) : arrayValue;
			}*/
			var arguments = Arguments.FromValue(controller);
			return Runtime.SendMessage(arrayValue, "sort", arguments);
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "sort";
	}
}