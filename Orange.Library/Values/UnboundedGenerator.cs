using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class UnboundedGenerator : Value, IGenerator
	{
		Value seed;
		Block nextValue;
		Value currentValue;

		public UnboundedGenerator(Value seed, Block nextValue)
		{
			this.seed = seed;
			this.nextValue = nextValue;
			this.nextValue.AutoRegister = false;
			currentValue = null;
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return currentValue == null ? seed.Text : currentValue.Text;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return currentValue?.Number ?? seed.Number;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Generator;

	   public override bool IsTrue => false;

	   public override Value Clone() => new UnboundedGenerator(seed.Clone(), (Block)nextValue.Clone());

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public void Before()
		{
		}

		public Value Next(int index)
		{
			if (currentValue == null)
			{
				currentValue = seed;
				return seed;
			}
			RegionManager.Regions.SetParameter("$0", currentValue);
			RegionManager.Regions.SetParameter(Runtime.VAR_MANGLE + "0", currentValue);
			currentValue = nextValue.Evaluate();
			return currentValue;
		}

		public void End()
		{
		}

		public override string ToString() => $"{seed} then {nextValue}";
	}
}