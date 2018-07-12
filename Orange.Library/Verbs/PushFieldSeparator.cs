using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class PushFieldSeparator : Verb
	{
		public override Value Evaluate() => Runtime.State.FieldSeparator;

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.NotApplicable;

	   public override string ToString() => ",";
	}
}