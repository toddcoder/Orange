using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class ContinuationParser : Parser
   {
      public ContinuationParser()
         : base($"^ /s* '...' {REGEX_END1} |tabs| /t")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         return new NullOp();
      }

      public override string VerboseName => "continuation";
   }
}