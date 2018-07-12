using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Any : Value
	{
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

		public override ValueType Type => ValueType.Any;

	   public override bool IsTrue => true;

	   public override Value Clone() => new Any();

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override string ToString() => "any";
	}
}