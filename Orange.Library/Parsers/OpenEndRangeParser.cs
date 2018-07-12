using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class OpenEndRangeParser : Parser
   {
      public OpenEndRangeParser()
         : base("^ |sp| '..*'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new OpenEndRange();
      }

      public override string VerboseName => "open end range";
   }
}