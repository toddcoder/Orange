using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class FunctionBodyParser : Parser
	{
		public FunctionBodyParser()
			: base(@"^\s*=")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			var parser = new AnyBlockParser();
			var index = position + length;
			bool isMacro;
			result.Value = parser.Parse(source, ref index, false, out isMacro);
			overridePosition = index;
			MultiCapable = !parser.SingleLine;
			return new NullOp();
		}

		public bool MultiCapable
		{
			get;
			set;
		}

		public override bool EndOfBlock
		{
			get
			{
				return false;//!MultiCapable;
			}
		}

		public override string VerboseName
		{
			get
			{
				return "Function body";
			}
		}
	}
}