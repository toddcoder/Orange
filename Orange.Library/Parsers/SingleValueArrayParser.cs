using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class SingleValueArrayParser : Parser
   {
      public SingleValueArrayParser()
         : base("^ |sp| '@'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new SingleValueArray();
      }

      public override string VerboseName => "single value array";
   }
}