using Orange.Library.Patterns;
using Orange.Library.Patterns2;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Patterns
{
	public class StringElementParser : Parser, IElementParser, IInstructionParser
	{
		public StringElementParser()
			: base("^ /s* /([quote])")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var parser = new StringParser();
			if (parser.Scan(source, position))
			{
				var text = parser.Value.Text;
				Element = new StringElement(text);
				Instruction = new StringInstruction(text);
				overridePosition = parser.Position;
				return new NullOp();
			}
			return null;
		}

		public override string VerboseName => "string element";

	   public Element Element
		{
			get;
			set;
		}

		public Instruction Instruction
		{
			get;
			set;
		}
	}
}