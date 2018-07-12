using System.Collections.Generic;

namespace Orange.Library.Values
{
	public class MemoLambda : Lambda
	{
		Memoizer memo;

		public MemoLambda(Region region, Block block, Parameters parameters, bool enclosing)
			: base(region, block, parameters, enclosing) => memo = new Memoizer();

	   protected override void evaluateArguments(Arguments arguments, out List<ParameterValue> values, out bool partial,
         bool register)
		{
			values = parameters.GetArguments(arguments);
			memo.Evaluate(values);
			Parameters.SetArguments(values);
			partial = false;
		}

		protected override Value evaluateBlock()
		{
			return memo.Evaluate(() => base.evaluateBlock());
		}
	}
}