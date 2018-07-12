using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class ImportParser : Parser
   {
      public ImportParser()
         : base("^ /(|tabs| 'import' /s+) /(.*?) /(/r /n | /r | /n | $)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var path = tokens[2];
         Color(position, tokens[1].Length, KeyWords);
         Color(path.Length, Strings);
         Color(tokens[3].Length, Whitespaces);

         return new Import(path);
      }

      public override string VerboseName => "import";
   }
}