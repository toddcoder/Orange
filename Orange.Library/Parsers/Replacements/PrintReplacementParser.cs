using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Replacements
{
   public class PrintReplacementParser : Parser, IReplacementParser
   {
      public PrintReplacementParser()
         : base("^ /s* '!'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[0].Length, Operators);
         Replacement = new PrintReplacement();
         return new NullOp();
      }

      public override string VerboseName => "print replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}