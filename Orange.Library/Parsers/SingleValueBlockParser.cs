using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class SingleValueBlockParser : Parser
	{
		ValueParser valueParser;
		BlockMessageInvokeParser messageInvokeParser;
		LambdaInvokeParser lambdaInvokeParser;

		public SingleValueBlockParser()
			: base(@"^\s*\^\s*")
		{
			valueParser = new ValueParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			var index = position + length;
			messageInvokeParser = new BlockMessageInvokeParser();
			if (messageInvokeParser.Scan(source, index))
			{
				overridePosition = messageInvokeParser.Result.Position;
				return new Push(messageInvokeParser.Result.Value);
			}
			lambdaInvokeParser = new LambdaInvokeParser();
			if (lambdaInvokeParser.Scan(source, index))
			{
				overridePosition = lambdaInvokeParser.Result.Position;
				return new Push(lambdaInvokeParser.Result.Value);
			}
			if (valueParser.Scan(source, index))
			{
				var block = new Block
				{
					new Push(valueParser.Result.Value)
				};
				overridePosition = valueParser.Result.Position;
				Result.Value = block;
				return new Push(block);
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "Single value block";
			}
		}
	}
}