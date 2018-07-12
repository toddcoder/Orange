using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Ternary : Value
	{
		public override int Compare(Value value) => new Boolean(Truth).Compare(value);

	   public override string Text
		{
			get
			{
				return new Boolean(Truth).Text;
			}
			set
			{
			}
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.Ternary;

	   public override bool IsTrue => Truth;

	   public override Value Clone() => new Ternary
		{
		   Truth = Truth,
		   Value = Value.Clone()
		};

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public bool Truth
		{
			get;
			set;
		}

		public Value Value
		{
			get;
			set;
		}

		public override string ToString() => Truth.ToString().ToLower();
	}
}