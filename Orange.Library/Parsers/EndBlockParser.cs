using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class EndBlockParser : Parser
	{
	   static string fixEndOfBlock(string pattern) => pattern.Length == 0 ? "" : $"^ /s* {pattern}";

	   bool consume;
		bool end;

		public EndBlockParser(string endOfBlock, bool consume)
			: base(fixEndOfBlock(endOfBlock))
		{
			this.consume = consume;
			end = endOfBlock.Length > 0;
		}

		public override Verb CreateVerb(string[] tokens)
		{
			if (!end)
				return null;

			Color(position, length, Structures);
			if (!consume)
				overridePosition = position;
			return new NullOp();
		}

		public override bool EndOfBlock => end;

	   public override string VerboseName => "end of block";
	}
}