using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class PrintlnParser : Parser
   {
      public PrintlnParser()
         : base("^ |tabs| 'println' (/r /n | /r | /n)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);

         return new Print(((String)"").Pushed, Print.PrintType.Print, true) { Index = position };
      }

      public override string VerboseName => "println";
   }
}