using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class SignalParser : Parser
	{
		AnyBlockParser anyBlockParser;

		public SignalParser()
			: base(@"^\s*(return|yield)\s+")
		{
			anyBlockParser = new AnyBlockParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[0].Length, IDEColor.EntityType.KeyWord);
			var type = tokens[1];
			var index = position + length;
			var block = anyBlockParser.Parse(source, ref index, false);
			overridePosition = index;
			result.Value = block;
			return new Signal(type, block);
		}

		public override string VerboseName
		{
			get
			{
				return "signal";
			}
		}
	}
}