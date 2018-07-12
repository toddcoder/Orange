using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class ScalarStream : ArrayStream
	{
		Double current;

		public ScalarStream(Value seed, int limit, Region region = null)
			: base(seed, null, region)
		{
			current = null;
			this.limit = limit;
		}

		public override Value Next()
		{
			using (var popper = new RegionPopper(region, "scalar-stream-next"))
			{
				popper.Push();
				if (current == null)
					current = (Double)seed.Number;
				else
				{
					if (current.Number >= limit)
						return new Nil();
					current = (Double)(current.Number + 1);
				}
				return current;
			}
		}

		public override Value Reset()
		{
			current = null;
			return this;
		}

		public override Value Clone() => new ScalarStream(seed.Clone(), limit);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "map", v => ((ScalarStream)v).Map());
			manager.RegisterMessage(this, "if", v => ((ScalarStream)v).IfMessage());
			manager.RegisterMessage(this, "take", v => ((ScalarStream)v).Take());
			manager.RegisterMessage(this, "next", v => ((ScalarStream)v).Next());
			manager.RegisterMessage(this, "reset", v => ((ScalarStream)v).Reset());
		}

		public override Value Map() => new Sequence(this)
		{
		   Arguments = Arguments.Clone()
		}.Map();

	   public override Value IfMessage() => new Sequence(this)
	   {
	      Arguments = Arguments.Clone()
	   }.If();

	   public override Value Take() => new Sequence(this)
	   {
	      Arguments = Arguments.Clone()
	   }.Take();
	}
}