using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class VariableReference : Value
	{
		const string LOCATION = "Variable reference";
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

		public override ValueType Type => ValueType.String;

	   public override bool IsTrue => false;

	   public override Value Clone() => null;

	   protected override void registerMessages(MessageManager manager)
		{
		}
	}
}