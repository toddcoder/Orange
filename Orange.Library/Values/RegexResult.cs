using System.Text;
using Orange.Library.Managers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
	public class RegexResult : Value
	{
		bool success;
		string input;
		Matcher.Match[] matches;

		public RegexResult(string input, Matcher.Match[] matches)
		{
			success = true;
			this.input = input;
			this.matches = matches;
		}

		public RegexResult()
		{
			success = false;
			input = "";
			matches = new Matcher.Match[0];
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.RegexResult;

	   public override bool IsTrue => success;

	   public override Value Clone() => success ? new RegexResult(input, matches) : new RegexResult();

	   protected override void registerMessages(MessageManager manager)
		{
		}

		static string visibleSpaces(string text) => text.Replace("\r", "µ").Replace("\n", "¶").Replace("\t", "¬").Replace(" ", "•");

	   public override string ToString()
		{
			if (success)
			{
				var result = new StringBuilder("%(");
				Slicer slicer = visibleSpaces(input);
				foreach (var match in matches)
				{
					slicer.Reset();
					slicer[match.Index, match.Length] = "§".Repeat(match.Length);
					for (var i = 1; i < match.Groups.Length; i++)
					{
						var group = match.Groups[i];
						slicer.Reset();
						slicer[group.Index, group.Length] = "‡".Repeat(group.Length);
					}
				}
				result.Append(slicer);
				result.Append(")");
				return result.ToString();
			}
			return "%(Failure)";
		}
	}
}