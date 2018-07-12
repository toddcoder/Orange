using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class FormatterParser : Parser
	{
		public FormatterParser()
			: base("^ /(['cdefgnprx'])? /('-'? /d+)? ('.' /(/d+))? $", true)
		{
			Width = "";
			Specifier = "";
			IndirectSpecifier = "";
		}

		public string Width
		{
			get;
			set;
		}

		public string Specifier
		{
			get;
			set;
		}

		public string IndirectSpecifier
		{
			get;
			set;
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Specifier = tokens[1] + tokens[3];
			Width = tokens[2];
			return new NullOp();
		}

		public override string VerboseName => "formatter parser";
	}
}