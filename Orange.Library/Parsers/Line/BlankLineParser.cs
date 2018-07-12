using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Line
{
   public class BlankLineParser : Parser
   {
      public BlankLineParser()
         : base("(^ /r /n) | (^ /r) | (^ /n)")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Whitespaces);
         return new BlankLine { Index = position };
      }

      public override string VerboseName => "skipped line";
   }
}