using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
	public class WordBoundaryElementParser : Parser, IElementParser
	{
		public WordBoundaryElementParser()
			: base("^ /s* /('<<' | '>>')")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Operators);
		   Element = tokens[1] == "<<" ? (Element)new WordLeftElement() : new WordRightElement();
			return new NullOp();
		}

		public override string VerboseName => "word boundary element";

	   public Element Element
		{
			get;
			set;
		}
	}
}