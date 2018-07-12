using Orange.Library.Templates;
using Orange.Library.Verbs;
using Print = Orange.Library.Templates.Print;

namespace Orange.Library.Parsers.Templates
{
	public class BlankLineTemplateParser : Parser, ITemplateItem
	{
		public BlankLineTemplateParser()
			: base(@"^!$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new Print("");
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "blank line template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}