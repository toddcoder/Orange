using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Nil : Value
	{
	   public static Nil NilValue => new Nil();

		public Nil() => PerformElse = null;

	   public Nil(bool ifTrue) => PerformElse = ifTrue;

	   public override int Compare(Value value) => value.Type == ValueType.Nil ? 0 : 1;

	   public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Nil;

	   public override bool IsTrue => false;

	   public override Value Clone() => this;

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override Value AlternateValue(string message) => null;

	   public override string ToString() => "nil";

	   public override bool IsNil => true;
	}
}