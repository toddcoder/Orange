using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class EndParser : Parser
   {
      public EndParser()
         : base(REGEX_END)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Comments);
         return new End();
      }

      public override string VerboseName => "end";
   }
}