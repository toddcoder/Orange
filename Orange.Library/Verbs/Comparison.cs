using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public abstract class Comparison : TwoValueVerb
	{
		public override Value Evaluate(Value x, Value y) => x.Type == Value.ValueType.Object ? MessageManager.MessagingState.SendMessage(x, "cmp", new Arguments(y)) : Compare(x.Compare(y));

	   public abstract bool Compare(int comparison);
	}
}