using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class PutTemplateParser : Parser, ITemplateItem
	{
		public PutTemplateParser()
			: base(@"^\.\s*(.*)$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new Put(tokens[1]);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "put template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}