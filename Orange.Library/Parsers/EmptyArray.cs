using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class EmptyArray : Parser
	{
		public EmptyArray()
			: base(@"^\s*<>")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Array);
			return new Push(new Array());
		}

		public override string VerboseName
		{
			get
			{
				return "empty array";
			}
		}
	}
}