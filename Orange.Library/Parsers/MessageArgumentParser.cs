using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MessageArgumentParser : Parser
   {
      public MessageArgumentParser()
         : base($"^ /(/s*) /({REGEX_VARIABLE}) /(':')")
      {

      }

      public override Verb CreateVerb(string[] tokens)
      {
         var messageName = tokens[2];
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Messaging);
         Color(tokens[3].Length, Structures);
         return new AppendToMessage(messageName);
      }

      public override string VerboseName => "Message argument";
   }
}