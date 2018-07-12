using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns2
{
	public class CharSet
	{
		Set<char> set;
		Set<char> upperSet;
		char[] chars;
		char[] upperChars;

		public CharSet(string source)
		{
			chars = Expand(source).ToCharArray();
			set = new Set<char>(chars);

			upperChars = chars.Select(char.ToUpper).ToArray();
			upperSet = new Set<char>(upperChars);
		}

		public bool Contains(char chr, bool ignoreCase) => ignoreCase ? upperSet.Contains(char.ToUpper(chr)) :
         set.Contains(chr);

	   public int IndexOf(string source, bool ignoreCase) => ignoreCase ? source.ToUpper().IndexOfAny(upperChars) :
         source.IndexOfAny(chars);

	   public override string ToString() => chars.Listify("");
	}
}