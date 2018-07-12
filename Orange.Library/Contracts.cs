using Orange.Library.Values;
using Standard.Types.Collections;

namespace Orange.Library
{
	public class Contracts
	{
		public static string Name(string possibleName)
		{
			string type;
			string name;

			if (Runtime.IsPrefixed(possibleName, out type, out name))
				return name;

			return possibleName;
		}

		Hash<string, Contract> contracts;

		public Contracts()
		{
			contracts = new Hash<string, Contract>();
		}

		public void RegisterContract(Contract contract)
		{
			contracts[contract.Name] = contract;
		}

		public void RegisterContract(Object obj, string name)
		{
			name = Name(name);
			contracts[name] = new Contract(obj, name);
			EnableContractMessage(name);
		}

		public void UnregisterContract(string name)
		{
			name = Name(name);
			contracts.Remove(name);
		}

		public bool ContractExists(string name)
		{
			name = Name(name);
			return contracts.ContainsKey(name);
		}

		public void EnableContractMessage(string name)
		{
			enableContractMessage(name, true);
		}

		void enableContractMessage(string message, bool enabled)
		{
/*			message = Name(message);
				contracts[plainName][message] = enabled;*/
		}

		public void DisableContractMessage(string message)
		{
			enableContractMessage(message, false);
		}

		public Contract Contract(string name)
		{
			return contracts[name];
		}

		public Contract ContractFromMessageName(string messageName)
		{
			string type;
			string plainName;

			return Runtime.IsPrefixed(messageName, out type, out plainName) ? contracts[plainName] : null;
		}

	}
}