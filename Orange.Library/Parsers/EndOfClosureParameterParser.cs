using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class EndOfClosureParameterParser : Parser
	{
		AnyBlockParser blockParser;

		public EndOfClosureParameterParser()
			: base(@"^\s*\)\s*=>\s*")
		{
			blockParser = new AnyBlockParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			var index = position + length;
			var block = blockParser.Parse(source, ref index, false);
			block.Stem = true;
			result.Value = block;
			overridePosition = index;
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "End of closure";
			}
		}
	}
}