using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class FunctionCall : Value
	{
		Lambda lambda;

		public FunctionCall(Lambda lambda) => this.lambda = lambda;

	   public override int Compare(Value value) => 0;

	   public override string Text
		{
			get;
			set;
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.FunctionCall;

	   public override bool IsTrue => false;

	   public override Value Clone() => new FunctionCall((Lambda)lambda.Clone());

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "eval", v => ((FunctionCall)v).Evaluate());
		}

		public Value Evaluate() => lambda.Evaluate(Arguments);

	   public override string ToString() => lambda.ToString();
	}
}