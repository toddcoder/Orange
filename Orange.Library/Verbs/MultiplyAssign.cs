using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class MultiplyAssign : OperatorAssign
	{
		public override Value Execute(Variable variable, Value value) => variable.Number * value.Number;

	   public override Value Exception(Variable variable, Value value)
		{
			var x = variable.Value;
			Value result = null;
			switch (x.Type)
			{
				case ValueType.String:
					if (value.IsNumeric())
						result = SendMessage(x, "repeat", value);
					else
						return null;
					break;
			}

			switch (value.Type)
			{
				case ValueType.String:
					if (x.IsNumeric())
						result = SendMessage(value, "repeat", x);
					else
						return null;
					break;
			}

			if (result == null)
				return null;
			variable.Value = result;
			return variable;
		}

		public override string Location => "Multiply assign";

	   public override string Message => "mult";

	   public override string ToString() => "*=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
	}
}