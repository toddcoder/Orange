using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class BlockMessageInvokeParser : Parser
	{
		SendMessageParser parser;

		public BlockMessageInvokeParser()
			: base(@"^(\s*)(" + Runtime.REGEX_VARIABLE + @")\.")
		{
			parser = new SendMessageParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespace);
			Color(tokens[2].Length, IDEColor.EntityType.Variable);
			var index = position + length - 1;
			if (parser.Scan(source, index))
			{
				var sendMessage = parser.Result.Verb;
				var block = new Block
				{
					new Push(new Variable(tokens[2])),
					sendMessage
				};
				result.Value = block;
				overridePosition = parser.Result.Position;
				return new NullOp();
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "Block message invoke";
			}
		}
	}
}