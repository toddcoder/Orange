using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Tuples;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;

namespace Orange.Library.Parsers
{
	public class ShortCircuitBooleanParser : Parser
	{
		public ShortCircuitBooleanParser()
			: base("^ /(/s*) /('and' | 'xor' | 'or') /(/s* '(')")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, Whitespaces);
			var operation = tokens[2];
			Color(operation.Length, KeyWords);
			Color(tokens[3].Length, Structures);
			var index = position + length;
		   Block block;
		   if (GetExpression(source, index, "')'").Assign(out block, out index))
		   {
		      Verb verb;
		      switch (operation)
		      {
		         case "and":
		            verb = new And();
		            break;
		         case "or":
		            verb = new Or();
		            break;
		         case "xor":
		            verb = new XOr();
		            break;
		         default:
		            return null;
		      }
		      result.Verbs = new List<Verb>
		      {
		         verb,
		         new Push(block)
		      };
		      overridePosition = index;
		      return new NullOp();
		   }
         return null;
		}

		public override string VerboseName => "Short-circuit";
	}
}