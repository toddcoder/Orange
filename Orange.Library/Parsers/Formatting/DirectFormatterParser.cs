using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Formatting
{
	public class DirectFormatterParser : Parser
	{
		public DirectFormatterParser()
			: base(@"^([a-zA-z])?(-?\d+)?(?:\.(\d+))?$", true)
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Specifier = tokens[1] + tokens[3];
			Width = tokens[2];
			return new NullOp();
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

		public override string VerboseName
		{
			get
			{
				return "direct format";
			}
		}
	}
}