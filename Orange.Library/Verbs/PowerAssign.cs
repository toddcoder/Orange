using Orange.Library.Values;
using static System.Math;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class PowerAssign : OperatorAssign
	{
		public override Value Execute(Variable variable, Value value) => Pow(variable.Number, value.Number);

	   public override string Location => "Power assign";

	   public override string Message => "pow";

	   public override string ToString() => "**=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
	}
}