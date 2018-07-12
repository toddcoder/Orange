using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Or : Verb
	{
	   Block expression;

	   public Or(Block expression) => this.expression = expression;

	   public Block Expression => expression;

	   public override string ToString() => "or";

	   public override Value Evaluate() => State.Stack.Pop(true, "Or").IsTrue || expression.Evaluate().IsTrue;

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Or;
	}
}