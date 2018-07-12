using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class XMLEndTemplateParser : Parser, ITemplateItem
	{
		public XMLEndTemplateParser()
			: base(@"^\s*%>")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new XMLEnd();
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "xml end template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}