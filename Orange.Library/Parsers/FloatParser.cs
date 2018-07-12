using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class FloatParser : Parser
	{
		public FloatParser()
			: base("^ |sp| ['+-']? /([/d '_']*) '.' /(/d+) /('e' ['+-']? /d+)? /(['ir'])?", true)
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			if (tokens[1] == "_")
				return null;
			Color(position, length, Numbers);
			var value = tokens[0].Replace("_", "").Substitute("['ir']", "").ToDouble();
			Value number;
			switch (tokens[4])
			{
				case "i":
					number = new Complex(0, value);
					break;
				case "r":
					number = new Rational((int)value, 1);
					break;
				default:
					number = new Double(value);
					break;
			}
			result.Value = number;
			return new Push(number);
		}

		public override string VerboseName => "float";
	}
}