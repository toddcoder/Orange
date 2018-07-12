using Orange.Library.Invocations;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class InvocationParser : Parser
	{
		public InvocationParser()
			: base("^ /(/s* '<[') (.*?)(\]>)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			int tokens1Length = tokens[1].Length;
			Color(position, tokens1Length, IDEColor.EntityType.Structure);
			string invocationSource = tokens[2];
			var invocation = new Invocation(invocationSource, position + tokens1Length);
			Color(tokens[3].Length, IDEColor.EntityType.Structure);
			Value value = new Invoker(invocation);
			result.Value = value;
			return new Push(value);
		}

		public override string VerboseName
		{
			get
			{
				return "invocation";
			}
		}
	}
}