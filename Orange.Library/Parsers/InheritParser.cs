using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class InheritParser : Parser
   {
      public InheritParser()
         : base("^ |tabs1| 'inherit' /b") {}

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         CurrentVisibility = Object.VisibilityType.Protected;
         return new NullOp();
      }

      public override string VerboseName => "inherit";
   }
}