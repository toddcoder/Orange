using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class XMLEndOfChildren : Parser
	{
		const string LOC_XML_ARRAY = "XML Array Parser";

		public XMLEndOfChildren()
			: base(@"^\s*%%>")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[0].Length, IDEColor.EntityType.Structures);
			return new NullOp();
		}

		public override bool EndOfBlock
		{
			get
			{
				return true;
			}
		}

		public override string VerboseName
		{
			get
			{
				return "xml end of children";
			}
		}
	}
}