using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class PadderEndTemplateParser : Parser, ITemplateItem
	{
		public PadderEndTemplateParser()
			: base(@"^\s*->")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new PadderEnd();
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "padder end";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}