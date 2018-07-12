using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Between : Value
	{
		const string LOC_BETWEEN = "Between";

		Block start;
		Block stop;
		bool set;

		public Between(Block start, Block stop)
		{
			this.start = start;
			this.stop = stop;
			set = false;
		}

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

		public override ValueType Type => ValueType.Between;

	   public override bool IsTrue
		{
			get
			{
				if (!set)
				{
					if (start.IsTrue)
					{
						set = true;
						return true;
					}
				}
				else
				{
					if (stop.IsTrue)
					{
						set = false;
						return true;
					}
				}
				return set;
			}
		}

		public override Value Clone() => new Between((Block)start.Clone(), (Block)stop.Clone());

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override Value AlternateValue(string message) => null;

	   public override string ToString() => start + ".." + stop;
	}
}