using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Values.Object.VisibilityType;

namespace Orange.Library.Parsers
{
   public class HideParser : Parser
   {
      public HideParser()
         : base("^ |tabs1| 'hide' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         CurrentVisibility = Private;
         return new NullOp();
      }

      public override string VerboseName => "hide";
   }
}