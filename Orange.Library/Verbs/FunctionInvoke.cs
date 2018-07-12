using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
	public class FunctionInvoke : Verb
	{
		string functionName;
		Arguments arguments;

		public FunctionInvoke(string functionName, Arguments arguments)
		{
			this.functionName = functionName;
			this.arguments = arguments;
		}

		public string FunctionName => functionName;

	   public Arguments Arguments => arguments;

	   public override Value Evaluate()
		{
			var value = Regions[functionName];
			return Invoke.Evaluate(value, arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Invoke;

	   public override string ToString() => $"{functionName}({arguments})";
	}
}