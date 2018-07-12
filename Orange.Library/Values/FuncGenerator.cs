using System;
using System.Collections.Generic;

namespace Orange.Library.Values
{
	public class FuncGenerator : Generator
	{
		Func<int, Value> next;
		Action before;
		Action after;

		public FuncGenerator(string parameterName, Func<int, Value> next, Action before = null, Action after = null)
			: base(parameterName, null)
		{
			this.next = next;
			this.before = before;
			this.after = after;
		}

		public FuncGenerator(string parameterName, Func<int, Value> next, List<Item> items, Action before = null, Action after = null)
			: base(parameterName, null, items)
		{
			this.next = next;
			this.before = before;
			this.after = after;
		}

	}
}