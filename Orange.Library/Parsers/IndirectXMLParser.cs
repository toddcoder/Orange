using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class IndirectXMLParser : Parser
	{
		public IndirectXMLParser()
			: base(@"^(\s*<\$\s*)(" + Runtime.REGEX_VARIABLE + @")(\s*\$>)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			string variableName = tokens[2];
			Color(position, tokens[1].Length, IDEColor.EntityType.Structure);
			Color(variableName.Length, IDEColor.EntityType.Variable);
			Color(tokens[3].Length, IDEColor.EntityType.Structure);
			result.Value = new IndirectXML(variableName);
			return new Push(result.Value);
		}

		public override string VerboseName
		{
			get
			{
				return "indirect xml";
			}
		}
	}
}