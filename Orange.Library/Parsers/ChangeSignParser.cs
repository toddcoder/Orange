using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class ChangeSignParser : Parser
   {
      public ChangeSignParser()
         : base("^ /s* '-' -(> /d)")
      { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new ChangeSign();
      }

      public override string VerboseName => "change sign";
   }
}