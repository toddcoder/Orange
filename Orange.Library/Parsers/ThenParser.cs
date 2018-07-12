using System.Collections.Generic;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class ThenParser : Parser
	{
		AnyBlockParser anyBlockParser;

		public ThenParser()
			: base(@"^\s*\then\s*")
		{
			anyBlockParser = new AnyBlockParser("");
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position,length,IDEColor.EntityType.KeyWord);
			var index = position + length;
			var block = anyBlockParser.Parse(source, ref index, false);
			result.Verbs = new List<Verb>
			{
				new Then(),
				new Push(block)
			};
			overridePosition = index;
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "then";
			}
		}
	}
}