using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class LazyOperatorParser : Parser
   {
      public LazyOperatorParser()
         : base("^ |sp| '..*'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new SendMessage("lazy", new Arguments());
      }

      public override string VerboseName => "lazy";
   }
}