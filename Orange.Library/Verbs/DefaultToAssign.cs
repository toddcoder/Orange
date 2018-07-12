using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class DefaultToAssign : OperatorAssign
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

	   public override Value Execute(Variable variable, Value value) => variable.Value.IsEmpty ? value : variable.Value;

	   public override string Location => "Default to assign";

	   public override string Message => "defaultTo";

	   public override string ToString() => "<>=";
	}
}