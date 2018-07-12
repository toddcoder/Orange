using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class XMLBeginTemplateParser : Parser, ITemplateItem
	{
		public XMLBeginTemplateParser()
			: base(@"^\s*<%\s*(\S+)(.*)?$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			string name = tokens[1];
			string attributes = tokens[2];
			Item = new XMLBegin(name, attributes);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "xml begin template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}