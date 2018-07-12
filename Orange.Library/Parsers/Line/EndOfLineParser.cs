using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Line
{
   public class EndOfLineParser : Parser
   {
      public EndOfLineParser()
         : base("(^ /r /n) | (^ /r) | (^ /n) | (^ ';') (> |tabs|)")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         return new End();
      }

      public override string VerboseName => "end of line";
   }
}