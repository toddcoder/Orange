using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Configurations;

namespace Orange.Library.Verbs
{
	public class CreateInitializer : Verb, ISerializeToGraph, IEnd
	{
		string functionName;
		Lambda lambda;

		public CreateInitializer(string functionName, Lambda lambda)
		{
			this.functionName = functionName;
			this.lambda = lambda;
		}

		public override Value Evaluate()
		{
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Push;
			}
		}

		public bool IsEnd
		{
			get;
			private set;
		}

		public bool EvaluateFirst
		{
			get;
			private set;
		}
	}
}