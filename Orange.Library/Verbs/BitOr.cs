using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class BitOr : TwoValueVerb
	{
		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.BitOr;

	   public override Value Evaluate(Value x, Value y)
		{
			return x.Type == Value.ValueType.Number && y.Type == Value.ValueType.Number ? (int)x.Number | (int)y.Number :
				Runtime.BitOperationOnText(x, y, (a, b) => a | b);
		}

		public override string Location => "Bit or";

	   public override string Message => "bor";

	   public override string ToString() => ".|";
	}
}