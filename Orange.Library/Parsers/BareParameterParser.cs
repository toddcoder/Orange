using Orange.Library.Values;
using Standard.Types.RegularExpressions;

namespace Orange.Library.Parsers
{
	public class BareParameterParser
	{
		const string REGEX_FIRST_VARIABLE = "^ /(/s*) /(" + Runtime.REGEX_VARIABLE + ")";
		const string REGEX_SECOND_VARIABLE = "^ /(/s* ',' /s*) /(" + Runtime.REGEX_VARIABLE + ")";
		const string REGEX_BRIDGE = "^ /s* '=>' /s*";

		Matcher matcher;
		string source;
		int index;
		bool matchBridge;
		bool matchedBridge;

		public BareParameterParser(string source, int index, bool matchBridge = false)
		{
			this.source = source;
			this.index = index;
			this.matchBridge = matchBridge;
			matchedBridge = false;
			matcher = new Matcher();
		}

		public Parameters Parse() => null;

	   public int Index => index;

	   public bool MatchedBridge => matchBridge && matchedBridge;
	}
}