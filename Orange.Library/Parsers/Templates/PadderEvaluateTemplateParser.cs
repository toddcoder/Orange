using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class PadderEvaluateTemplateParser : Parser, ITemplateItem
	{
		public PadderEvaluateTemplateParser()
			: base(@"^\s*-\s*(.*)$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			string text = tokens[1];
			Item = new PadderEvaluate(text);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "padder evaluate";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}