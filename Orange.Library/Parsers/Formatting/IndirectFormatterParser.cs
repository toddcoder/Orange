using Orange.Library.Values;
using Orange.Library.Verbs;
using Format = Orange.Library.Verbs.Format;

namespace Orange.Library.Parsers.Formatting
{
	public class IndirectFormatterParser : Parser
	{
		public IndirectFormatterParser()
			: base(@"^(\s*@#?)(-)?(" + Runtime.REGEX_VARIABLE + ")")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position,tokens[1].Length,IDEColor.EntityType.Verb);
			string sign = tokens[2];
			Color(sign.Length, IDEColor.EntityType.Verb);
			string variable = tokens[3];
			Color(variable.Length, IDEColor.EntityType.Variable);
			return new Format(new Variable(variable), sign);
		}

		public override string VerboseName
		{
			get
			{
				return "indirect formatter";
			}
		}
	}
}