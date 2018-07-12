using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Replacements
{
   public class AssignToAnsReplacementParser : Parser, IReplacementParser
   {
      public AssignToAnsReplacementParser()
         : base("^ /(/s* '@'? '.')")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         Replacement = tokens[1].Trim() == "." ? (IReplacement)new AssignToAnsReplacement() : new PushToAnsReplacement();
         return new NullOp();
      }

      public override string VerboseName => "Assign to $ans replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}