using Orange.Library.Conditionals;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Conditionals
{
	public class ConditionalParser : Parser
	{
		public ConditionalParser()
			: base("^ /(/s* ':') '{'")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var lengthLessBracket = length - 1;
			Color(position, lengthLessBracket, IDEColor.EntityType.Operators);
			var parser = new LambdaParser();
			if (parser.Scan(source, position + lengthLessBracket))
			{
				Conditional = new Conditional((Lambda)parser.Result.Value);
				overridePosition = parser.Result.Position;
			}
			else
				Conditional = new Unconditional();
			return new NullOp();
		}

		public override void FailedScan(string source, int position) => Conditional = new Unconditional();

	   public override string VerboseName => "pattern element conditional";

	   public Conditional Conditional
		{
			get;
			set;
		}
	}
}