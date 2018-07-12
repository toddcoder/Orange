using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class RationalParser : Parser
	{
		public RationalParser()
			: base("^ /s* /(['+-']? [/d '_']+) '/' /(['+-']? [/d '_']+)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Numbers);
			int numerator = tokens[1].ToInt();
			int denominator = tokens[2].ToInt();
			Runtime.Assert(denominator != 0, "Rational Parser", "Denominator can't be 0");
			return new Push(new Rational(numerator, denominator));
		}

		public override string VerboseName
		{
			get
			{
				return "Rational";
			}
		}
	}
}