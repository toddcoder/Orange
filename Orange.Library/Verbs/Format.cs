using Orange.Library.Values;
using Standard.Types.RegularExpressions;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class Format : Verb
	{
		const string REGEX_FORMAT = "^ /['cdefgnprxs'] /('-'? /d+)? ('.' /(/d*))? $";
		const string LOCATION = "Format";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var format = stack.Pop(true, LOCATION);
			var value = stack.Pop(true, LOCATION, false);
			switch (value.Type)
			{
				case ValueType.Object:
				case ValueType.Rational:
					return SendMessage(value, "fmt", format);
			}
			if (format.IsArray)
				return MessagingState.SendMessage(value, "format", new Arguments(format.SourceArray));
			var matcher = new Matcher();
			var input = format.Text;
			if (matcher.IsMatch(input, REGEX_FORMAT))
			{
				var formatter = new Formatter(matcher.FirstGroup, matcher.SecondGroup, matcher.ThirdGroup);
				return formatter.Format(value.Text);
			}
			switch (value.Type)
			{
				case ValueType.Number:
					return value.Number.ToString(input);
				case ValueType.Date:
					return ((Date)value).DateTime.ToString(input);
			}
			var formatString = "{0:\"" + input + "\"}";
			return string.Format(formatString, ConvertToObject(value.Text, input.EndsWith("x}")));
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Format;

	   public override string ToString() => "\\";
	}
}