using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class AddAssign : OperatorAssign
	{
		public override Value Execute(Variable variable, Value value) => variable.Number + value.Number;

	   public override Value Exception(Variable variable, Value value)
		{
			var x = variable.Value;
			if (x.Type == ValueType.Object)
				return null;

			if (!x.IsNumeric() && !value.IsNumeric())
			{
				var result = x.Text + value.Text;
				variable.Value = result;
				return variable;
			}
			return null;
		}

		public override string Location => "Add assign";

	   public override string Message => "add";

	   public override string ToString() => "+=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
	}
}