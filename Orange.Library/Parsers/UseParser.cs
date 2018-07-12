using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class UseParser : Parser
   {
      public UseParser()
         : base($"^ /(|tabs| 'use' /s+) /({REGEX_VARIABLE})")
      { }

      public override Verb CreateVerb(string[] tokens)
      {
         var moduleName = tokens[2];
         Color(position, tokens[1].Length, KeyWords);
         Color(moduleName.Length, Variables);
         return new Use(moduleName);
      }

      public override string VerboseName => "use";
   }
}