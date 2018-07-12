using System;
using Orange.Library.Values;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using Object = Orange.Library.Values.Object;

namespace Orange.Library
{
	public class Contract
	{
		[Flags]
		public enum ContractType
		{
			None = 0,
			Require = 1,
			Ensure = 2,
			Before = 4,
			After = 8,
			Invariant = 16
		}

		const string LOCATION = "Contract";

		Object obj;
		string name;

		Bits32<ContractType> types;

		public Contract(Object obj, string name)
		{
			this.obj = obj;
			this.name = name;
			types = ContractType.None;
		}

		public Contract(Object obj, string name, Bits32<ContractType> types)
			: this(obj, name)
		{
			this.types = types;
		}

		public static ContractType MessageToContractType(string messageName)
		{
			string type;
			string plainName;
			if (Runtime.IsPrefixed(messageName, out type, out plainName))
			{
				var contractType = type.ToEnumeration<ContractType>();
				return contractType;
			}
			return ContractType.None;
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public static ContractType TypeToContractType(string type)
		{
			switch (type)
			{
				case "aft":
					return ContractType.After;
				case "bef":
					return ContractType.Before;
				case "req":
					return ContractType.Require;
				case "ens":
					return ContractType.Ensure;
				case "inv":
					return ContractType.Invariant;
				default:
					return ContractType.None;
			}
		}

		public bool this[string messageName]
		{
			get
			{
				var contractType = MessageToContractType(messageName);
				return types[contractType];
			}
			set
			{
				var contractType = MessageToContractType(messageName);
				types[contractType] = value;
			}
		}

		public Value OldValue
		{
			get;
			set;
		}

		public void Require(Arguments arguments)
		{
			if (!types[ContractType.Require])
				return;

			var messageName = Runtime.FunctionPrefix("require", name);
			if (!obj.RespondsTo(messageName))
				return;

			var result = obj.SendToSelf(messageName, arguments);
			Runtime.Assert(result.IsTrue, LOCATION, "Requirement for {0} failed", name);
		}

		public void Ensure(Value value)
		{
			if (!types[ContractType.Ensure])
				return;

			var messageName = Runtime.FunctionPrefix("ensure", name);

			if (!obj.RespondsTo(messageName))
				return;

			var result = obj.SendToSelf(messageName, value);
			Runtime.Assert(result.IsTrue, LOCATION, "Ensurement for {0} failed", name);
		}

		public void Before(Arguments arguments)
		{
			if (!types[ContractType.Before])
				return;

			var value = arguments[0];
			Before(value);
		}

		public void Before(Value value)
		{
			if (!types[ContractType.Before])
				return;

			var messageName = Runtime.FunctionPrefix("after", name);
			if (obj.RespondsTo(messageName))
			{
				var getterName = Runtime.FunctionPrefix("get", name);
				OldValue = obj.SendToSelf(getterName);
			}

			messageName = Runtime.FunctionPrefix("before", name);
			if (!obj.RespondsTo(messageName))
				return;

			obj.SendToSelf(messageName, value);
		}

		public void After(Arguments arguments)
		{
			if (!types[ContractType.After] && !types[ContractType.Invariant])
				return;

			var value = arguments[0];
			After(value);
		}

		public void After(Value value)
		{
			if (!types[ContractType.After] && !types[ContractType.Invariant])
				return;

			string messageName;
			var arguments = new Arguments(new[]
			{
				OldValue,
				value
			});
			if (types[ContractType.After])
			{
				messageName = Runtime.FunctionPrefix("after", name);
				if (obj.RespondsTo(messageName))
				{
					obj.SendToSelf(messageName, arguments);
				}
			}

			if (!types[ContractType.Invariant])
				return;

			messageName = Runtime.FunctionPrefix("invariant", name);
			if (!obj.RespondsTo(messageName))
				return;
			var result = obj.SendToSelf(messageName, arguments);
			Runtime.Assert(result.IsTrue, LOCATION, "Invariant for {0} failed", name);
		}
	}
}