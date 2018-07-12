using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class AlternateElementParser : Parser
   {
      public AlternateElementParser()
         : base("^ /s* '|'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[0].Length, Structures);
         return new NullOp();
      }

      public override string VerboseName => "alternate pattern element";
   }
}