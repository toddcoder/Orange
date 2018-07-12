using Standard.Types.Collections;

namespace Orange.Library
{
	public class VariableNames
	{
		AutoHash<int, string> hash;

		public VariableNames(string[] variableNames)
		{
		   hash = new AutoHash<int, string>("");
			for (var i = 0; i < variableNames.Length; i++)
				hash[i] = variableNames[i];
		}

		public string VariableName(int index, string defaultValue)
		{
			hash.DefaultValue = defaultValue;
			return hash[index];
		}
	}
}