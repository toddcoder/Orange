using System;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static System.DateTime;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
	public class DateParser : Parser
	{
		public DateParser()
			: base("^ ' '* '#(' /(-[')']+) ')'")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var dateSource = tokens[1];
			Color(position, length, EntityType.Dates);
			DateTime date;
			Assert(TryParse(dateSource, out date), "Date parser", $"Didnt' recognize {dateSource} as a date");
			return new Push(new Date(date));
		}

		public override string VerboseName => "date";
	}
}