using System;
using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class RangeGenerator : IGenerator
	{
		Value start;
		Func<Value, Value> increment;
		Value current;
		bool stopped;

		public RangeGenerator(Value start, Func<Value, Value> increment)
		{
			this.start = start;
			this.increment = increment;
			current = null;
			stopped = false;
		}

		public void Before()
		{
		}

		public Value Next(int index)
		{
			if (stopped)
				return new Nil();
			if (current == null)
			{
				current = start;
				return current;
			}
			current = increment(current);
			if (current.IsNil)
				stopped = true;
			return current;
		}

		public void End()
		{
		}
	}
}