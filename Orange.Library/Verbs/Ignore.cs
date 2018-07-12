using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Ignore : Verb
	{
		public override Value Evaluate() => null;

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Push;
	}
}