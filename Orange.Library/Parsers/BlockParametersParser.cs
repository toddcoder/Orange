using System.Collections.Generic;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;

namespace Orange.Library.Parsers
{
	public class BlockParametersParser : Parser
	{
		int maxCount;
		int offset;

		public BlockParametersParser(int maxCount, int offset)
			: base("^ /(-['|']+) '|'")
		{
			this.maxCount = maxCount;
			this.offset = offset;
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var list = new List<string>();
			var index = 0;

			var matcher = new Matcher();
			if (matcher.IsMatch(tokens[1], "^ /s*"))
			{
				var whitespaceLength = matcher[0].Length;
				Color(offset + index, whitespaceLength, IDEColor.EntityType.Whitespace);
				index = whitespaceLength;
			}
			if (!matcher.IsMatch(tokens[1], "/(" + Runtime.REGEX_VARIABLE + ") /(/s*)", true))
				return null;
			for (var i = 0; i < matcher.MatchCount; i++)
			{
				var variableName = matcher[i, 1];
				list.Add(variableName);
				Color(offset+ index, variableName.Length, IDEColor.EntityType.Variable);
				Color(matcher[i, 2].Length, IDEColor.EntityType.Whitespace);
				index += matcher[i].Length;
			}
			Color(1, IDEColor.EntityType.Structure);
			while (list.Count > maxCount)
				list.RemoveAt(list.Count - 1);
			Variables = list.ToArray();
			while (list.Count < maxCount)
				list.Add("");
			overridePosition = offset + index + 1;
			return new NullOp();
		}

		public override string VerboseName => "block parameter";

	   public string[] Variables
		{
			get;
			set;
		}
	}
}