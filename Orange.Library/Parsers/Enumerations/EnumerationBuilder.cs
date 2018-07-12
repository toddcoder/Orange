using Orange.Library.Values;
using Standard.Types.Collections;

namespace Orange.Library.Parsers.Enumerations
{
	public class EnumerationBuilder
	{
		Hash<string, int> values;
		int currentValue;

		public EnumerationBuilder()
		{
			values = new Hash<string, int>();
			currentValue = 0;
		}

		public void Add(string name) => values[name] = currentValue++;

	   public void Add(string name, int value)
		{
			currentValue = value;
			Add(name);
		}

		public Value Enumeration() => new Enumeration(values);
	}
}