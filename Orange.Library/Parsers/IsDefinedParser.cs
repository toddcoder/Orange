using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class IsDefinedParser : Parser
   {
      public IsDefinedParser()
         : base($"^ /(' ' '?') /({REGEX_VARIABLE})") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Operators);
         var variableName = tokens[2];
         Color(variableName.Length, Variables);
         return new IsDefined(variableName);
      }

      public override string VerboseName => "is defined";
   }
}