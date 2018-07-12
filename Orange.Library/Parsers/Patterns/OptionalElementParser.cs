using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class OptionalElementParser : Parser
   {
      public OptionalElementParser()
         : base("^ /s* '?'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[0].Length, Operators);
         return new NullOp();
      }

      public override string VerboseName => "optional pattern element";
   }
}