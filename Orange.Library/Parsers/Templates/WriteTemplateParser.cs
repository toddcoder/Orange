using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class WriteTemplateParser : Parser, ITemplateItem
	{
		public WriteTemplateParser()
			: base(@"^:\s*(.*)$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new Write(tokens[1]);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "write template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}