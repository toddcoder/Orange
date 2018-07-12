using Orange.Library.Templates;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Templates
{
	public class CodeTemplateParser : Parser, ITemplateItem
	{
		public CodeTemplateParser()
			: base(@"^\s*%\s*(?!>)(.*)$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Item = new Code(tokens[1]);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "code template";
			}
		}

		public Item Item
		{
			get;
			set;
		}
	}
}