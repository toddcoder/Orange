using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class XMLTextTemplateParser : Parser, ITemplateItem
	{
		public XMLTextTemplateParser()
			: base(@"^\s*=\s*(.*)$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new XMLText(tokens[1]);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "xml text template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}