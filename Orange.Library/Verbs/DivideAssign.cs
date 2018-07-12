using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class DivideAssign : OperatorAssign
	{
		public override Value Execute(Variable variable, Value value) => variable.Number / value.Number;

	   public override string Location => "Divide assign";

	   public override string Message => "div";

	   public override string ToString() => "/=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
	}
}