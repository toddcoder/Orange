using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor;

namespace Orange.Library.Parsers.Enumerations
{
   public class EndEnumerationParser : Parser
   {
      public EndEnumerationParser()
         : base("^ /s* '}'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, EntityType.Structures);
         return new NullOp();
      }

      public override string VerboseName => "end enumeration";

      public override bool EndOfBlock => true;
   }
}