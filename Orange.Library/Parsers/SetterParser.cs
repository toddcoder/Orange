using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static System.Activator;
using static Orange.Library.Parsers.TwoCharacterOperatorParser;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class SetterParser : Parser
   {
      static IMaybe<Block> combineOperation(string message, string op, Block expression)
      {
         var type = Operator(op);
         if (type == null)
            return none<Block>();

         var verb = (Verb)CreateInstance(type);
         var builder = new CodeBuilder();
         builder.SendMessage(GetterName(message), new Arguments());
         builder.Verb(verb);
         builder.Parenthesize(expression);
         return builder.Block.Some();
      }

      public SetterParser()
         : base($" ^ /(|sp| '.') /({REGEX_VARIABLE}) /(|sp|) {REGEX_ASSIGN}") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var message = tokens[2];

         Color(position, tokens[1].Length, Structures);
         Color(message.Length, Messaging);
         Color(tokens[3].Length, Whitespaces);

         var assignParser = new AssignParser();
         var start = position + tokens[1].Length + message.Length + tokens[3].Length;
         if (assignParser.Parse(source, start).If(out var assignment, out var index))
         {
            overridePosition = index;
            return new Setter(message, assignment.Verb, assignment.Expression) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "setter";
   }
}