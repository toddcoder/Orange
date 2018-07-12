using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class NotImplemented : Verb
	{
		const string LOCATION = "Not implemented";

		public override Value Evaluate()
		{
			Runtime.Throw(LOCATION, Runtime.State.Block.ID.ToString());
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Push;
			}
		}

		public override string ToString()
		{
			return "...";
		}
	}
}