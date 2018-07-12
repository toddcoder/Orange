using System.Collections.Generic;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Orange.Library.Verbs
{
	public static class VerbHelper
	{
		 public static string Representation(IEnumerable<Verb> verbs)
		 {
			 return verbs.Listify(" ");
		 }
	}
}