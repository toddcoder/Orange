using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class NullOpParser : Parser
   {
      public NullOpParser()
         : base("'`' [/r /n]+")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         return new NullOp();
      }

      public override string VerboseName => "null op";
   }
}