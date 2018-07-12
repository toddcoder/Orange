using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class PlainSignalParser : Parser
   {
      public PlainSignalParser()
         : base("^ |tabs| /('exit' | 'continue') /b")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         var type = tokens[1];
         return new PlainSignal(type);
      }

      public override string VerboseName => "signal";
   }
}