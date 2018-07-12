namespace Orange.Library.Values
{
	public class PseudoStream : ArrayStream
	{
		Array array;
		int index;

		public PseudoStream(Array array, Region region = null)
			: base(array[0], null, region)
		{
			this.array = array;
			index = -1;
			limit = array.Length;
		}

		public override Value Next()
		{
			using (var popper = new RegionPopper(region, "pseudo-stream-next"))
			{
				popper.Push();
				if (++index < array.Length)
				{
					var current = array[index];
					if (evaluate(ifBlock, current).IsTrue)
						return current;
				}
				return new Nil();
			}
		}

		public override Value Reset()
		{
			index = -1;
			return this;
		}

		public override Value Clone() => new PseudoStream(array);

	   public override Value AlternateValue(string message) => array;
	}
}