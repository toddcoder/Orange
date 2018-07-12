using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class PadderBeginTemplateParser : Parser, ITemplateItem
	{
		public PadderBeginTemplateParser()
			: base(@"^\s*<-\s*(.*)$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			string text = tokens[1];
			Item = new PadderBegin(text);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "padder begin";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}