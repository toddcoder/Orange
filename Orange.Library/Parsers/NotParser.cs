using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class NotParser : Parser
   {
      public NotParser()
         : base("^ |sp| 'not' /b -(> /s+ 'in' /b)")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         return new Not();
      }

      public override string VerboseName => "not";
   }
}