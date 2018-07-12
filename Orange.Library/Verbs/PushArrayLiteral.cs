using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class PushArrayLiteral : Verb
	{
		Array literal;

		public PushArrayLiteral(Array literal) => this.literal = literal;

	   public override Value Evaluate() => literal.Clone();

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => literal.ToString();

	   public Array Array => literal;
	}
}