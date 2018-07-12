using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Replacements
{
   public class TestReplacementParser : Parser, IReplacementParser
   {
      public TestReplacementParser()
         : base("^ /s* '?' /('.')")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length - 1, Operators);
         Color(1, Strings);
         var character = tokens[1];
         Replacement = new TestReplacement(character);
         return new NullOp();
      }

      public override string VerboseName => "test replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}