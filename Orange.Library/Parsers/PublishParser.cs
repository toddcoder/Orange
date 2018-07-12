using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Values.Object.VisibilityType;

namespace Orange.Library.Parsers
{
   public class PublishParser : Parser
   {
      public PublishParser()
         : base("^ |tabs1| 'publish' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         CurrentVisibility = Public;
         return new NullOp();
      }

      public override string VerboseName => "publish";
   }
}