using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class SomeOperatorParser : Parser
   {
      public SomeOperatorParser()
         : base("^ |sp| '?' -(> '?')")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new SomeOp();
      }

      public override string VerboseName => "some op";
   }
}