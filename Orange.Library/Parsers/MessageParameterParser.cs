using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MessageParameterParser : Parser
   {
      string messageName;
      string parameterName;

      public MessageParameterParser()
         : base($"^ /(/s+) /({REGEX_VARIABLE}) /(':' /s*) /({REGEX_VARIABLE})") { }

      public override Verb CreateVerb(string[] tokens)
      {
         messageName = tokens[2];
         parameterName = tokens[4];
         Color(position, tokens[1].Length, Whitespaces);
         Color(messageName.Length, Messaging);
         Color(tokens[3].Length, Structures);
         Color(parameterName.Length, Variables);
         return new NullOp();
      }

      public override string VerboseName => "message parameter";

      public string MessageName => messageName;

      public string ParameterName => parameterName;
   }
}