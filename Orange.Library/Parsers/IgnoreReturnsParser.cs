using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class IgnoreReturnsParser : Parser
   {
      public IgnoreReturnsParser()
         : base("^ |sp| '...' (/r /n | /r | /n) /s*")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Whitespaces);
         return new NullOp();
      }

      public override string VerboseName => "ignore returns";
   }
}