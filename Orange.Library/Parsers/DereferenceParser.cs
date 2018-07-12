using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class DereferenceParser : Parser
   {
      public DereferenceParser()
         : base("^ |sp| '&'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new Dereference();
      }

      public override string VerboseName => "dereference";
   }
}