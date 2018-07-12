using System.Collections.Generic;

namespace Orange.Library.Values
{
	public class LambdaApp : Lambda
	{
		List<Lambda> lambdas;

		public LambdaApp(Lambda left, Lambda right) => lambdas = new List<Lambda>
		{
		   left,
		   right
		};

	   public void Add(Lambda lambda)
		{
			lambdas.Add(lambda);
		}

		public override Value Evaluate(Arguments arguments, Value instance = null, bool register = true, bool setArguments = true)
		{
			Value result = null;
			for (var i = lambdas.Count - 1; i > -1; i--)
			{
				var lambda = lambdas[i];
				var argumentsToUse = result == null ? arguments : new Arguments(result);
				result = lambda.Evaluate(argumentsToUse, instance, register, setArguments);
				if (result.IsNil)
					return result;
			}
			return result;
		}
	}
}