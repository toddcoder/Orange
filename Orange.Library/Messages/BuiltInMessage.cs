using System;
using Orange.Library.Values;

namespace Orange.Library.Messages
{
	public class BuiltInMessage : Message
	{
		Func<Value, Value> func;

		public BuiltInMessage(Func<Value, Value> func, bool resolveArguments = true)
		{
			this.func = func;
			ResolveArguments = resolveArguments;
		}

		public override Value Evaluate(Value value) => func(value);
	}
}