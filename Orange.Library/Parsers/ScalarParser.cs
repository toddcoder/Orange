using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Tuples;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.TwoCharacterOperatorParser;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
	public class ScalarParser : Parser
	{
		public ScalarParser()
			: base(@"^ /(/s* '-' [':/*'] /s*) /(['+*//%=!<>&|@#~.,\^?:`-'] 1%3)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var type = Operator(tokens[2]);
			if (type == null)
				return null;
			Color(position, length, Operators);
			bool leftToRight;
		   Block block;
		   OperatorBlock(type).Assign(out block, out leftToRight);
			string messageName;
			switch (tokens[1].Trim())
			{
				case "-:":
					messageName = leftToRight ? "scalar" : "rscalar";
					break;
				case "-/":
					messageName = leftToRight ? "reduce" : "rreduce";
					break;
				case "-*":
					messageName = "zip";
					return new Cross(block, messageName);
				default:
					return null;
			}

			var message = new Message(messageName, new Arguments(new NullBlock(), block));
			return new ApplyToMessage(message);
		}

		public override string VerboseName => "Scalar";
	}
}