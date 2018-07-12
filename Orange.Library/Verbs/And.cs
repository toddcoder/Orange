using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class And : Verb
	{
	   Block expression;

	   public And(Block expression) => this.expression = expression;

	   public override string ToString() => "and";

	   public override Value Evaluate() => State.Stack.Pop(true, "And").IsTrue && expression.Evaluate().IsTrue;

	   public override VerbPresidenceType Presidence => VerbPresidenceType.And;
	}
}