using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class RangeOperatorParser : Parser
   {
      public RangeOperatorParser()
         : base("^ ' '* '$'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new SendMessage("range", new Arguments());
      }

      public override string VerboseName => "range operator";
   }
}