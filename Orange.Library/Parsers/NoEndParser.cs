using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class NoEndParser : Parser
   {
      public NoEndParser()
         : base("^ /'...' /(/r /n | /r | /n)")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Structures);
         Color(tokens[2].Length, Whitespaces);
         return new NullOp();
      }

      public override string VerboseName => "No end";
   }
}