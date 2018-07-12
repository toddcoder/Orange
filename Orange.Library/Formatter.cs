using System.Text;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library
{
	public class Formatter
	{
		const string LOCATION = "Formatter";

		public static Formatter Parse(string source)
		{
			var matcher = new Matcher();
			if (matcher.IsMatch(source, "/['cdefgnprxs'] /('-'? /d+)? ('.' /(/d+))?", true))
			{
				var specifier = matcher.FirstGroup;
				var width = matcher.SecondGroup;
				var places = matcher.ThirdGroup;
				return new Formatter(specifier, width, places);
			}
			Throw(LOCATION, $"Didn't understand formatter specification {source}");
			return null;
		}

		string format;

		public Formatter(string specifier, string width, string places)
		{
			var result = new StringBuilder("{0");
			if (width.IsNotEmpty())
				result.Append($",{width}");
			if (specifier.IsNotEmpty() && specifier != "s")
			{
				result.Append($":{specifier}");
				if (places.IsNotEmpty())
					result.Append(places);
			}
			result.Append("}");
			format = result.ToString();
		}

		public string Format(string text)
		{
			var obj = ConvertToObject(text, format.IsMatch("'d' | 'x}'"));
			return string.Format(format, obj);
		}
	}
}