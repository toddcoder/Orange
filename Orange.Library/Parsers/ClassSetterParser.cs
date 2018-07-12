using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class ClassSetterParser : Parser
   {
      public ClassSetterParser()
         : base($"^ /(|sp|) /'@' /({REGEX_VARIABLE}) /(|sp|) {REGEX_ASSIGN}") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var message = tokens[2];

         Color(position, tokens[1].Length, Whitespaces);
         Color(1, Structures);
         Color(message.Length, Messaging);
         Color(tokens[3].Length, Whitespaces);

         var assignParser = new AssignParser();
         var start = position + tokens[1].Length + 1 + message.Length + tokens[3].Length;
         if (assignParser.Parse(source, start).If(out var assigment, out var index))
         {
            overridePosition = index;
            return new ClassSetter(message, assigment.Verb, assigment.Expression);
         }

         return null;
      }

      public override string VerboseName => "class setter";
   }
}