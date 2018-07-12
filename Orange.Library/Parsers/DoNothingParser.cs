using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class DoNothingParser : Parser
   {
      public DoNothingParser()
         : base("^ |tabs| 'pass'")
      {

      }
      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         return new DoNothing();
      }

      public override string VerboseName => "do nothing";
   }
}