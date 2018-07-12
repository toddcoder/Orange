using Orange.Library.Values;

namespace Orange.Library
{
	public class ContractEnforcer
	{
		const string LOCATION = "Contract enforcer";

		Object obj;
		string type;
		string name;
		bool isSetter;
		bool disabled;

		public ContractEnforcer(Object obj, string message)
		{
			this.obj = obj;
			if (Runtime.IsPrefixed(message, out type, out name))
				isSetter = type == "set";
			else
			{
				type = "";
				name = message;
				isSetter = false;
			}
			OldValue = new Nil();
			switch (type)
			{
				case "get":
				case "set":
					disabled = false;
					break;
				default:
					disabled = true;
					break;
			}
		}

		public Value OldValue
		{
			get;
			set;
		}

		public void EnforceRequirement(Arguments arguments)
		{
			if (disabled)
				return;

			var message = Runtime.FunctionPrefix("require", name);

			if (!obj.RespondsTo(message))
				return;

			var result = obj.SendToSelf(message, arguments);
			Runtime.Assert(result.IsTrue, LOCATION, "Requirement for {0} failed", name);
		}

		public void EnforceEnsurement(Value result)
		{
			if (disabled)
				return;

			var message = Runtime.FunctionPrefix("ensure", name);

			if (!obj.RespondsTo(message))
				return;

			var result2 = obj.SendToSelf(message, result);
			Runtime.Assert(result2.IsTrue, LOCATION, "Ensurement for {0} failed", name);
		}

		public void InvokeBefore(Value value)
		{
			if (!isSetter || disabled)
				return;

			var afterName = Runtime.FunctionPrefix("after", name);
			if (obj.RespondsTo(afterName))
			{
				var getterName = Runtime.FunctionPrefix("get", name);
				OldValue = obj.SendToSelf(getterName);
			}

			var beforeName = Runtime.FunctionPrefix("before", name);
			if (obj.RespondsTo(beforeName))
				obj.SendToSelf(beforeName, value);
		}

		public void InvokeBefore(Arguments arguments)
		{
			if (!isSetter || disabled)
				return;

			var value = arguments[0];
			InvokeBefore(value);
		}

		public void InvokeAfter(Value value)
		{
			if (!isSetter || disabled)
				return;

			var afterName = Runtime.FunctionPrefix("after", name);
			if (obj.RespondsTo(afterName))
				obj.SendToSelf(afterName, OldValue);

			var invariantName = Runtime.FunctionPrefix("invariant", name);
			if (!obj.RespondsTo(invariantName))
				return;

			var result = obj.SendToSelf(invariantName, value);
			Runtime.Assert(result.IsTrue, LOCATION, "Invariant for {0} failed", name);
		}

		public void InvokeAfter(Arguments arguments)
		{
			if (!isSetter || disabled)
				return;

			var value = arguments[0];
			InvokeAfter(value);
		}
	}
}