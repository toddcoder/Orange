using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class OptionParser : Parser
	{
		Matcher matcher;

		public OptionParser()
			: base(@"^\s*:\[")
		{
			matcher = new Matcher();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Array);
			int index = position + length;
			if (matcher.IsMatch(source.Substring(index), @"^([^]]+)\]"))
			{
				string options = matcher[0, 1];
				int optionsLength = options.Length + 1;
				Color(optionsLength, IDEColor.EntityType.Array);
				overridePosition = index + optionsLength;
				return new SetOptions(options.Split(@"\s+"));
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "options";
			}
		}
	}
}