using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Replacements
{
   public class AtReplacementParser : Parser, IReplacementParser
   {
      public AtReplacementParser()
         : base($"^ /(/s* '@')? /(/s* '=') /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Operators);
         Color(tokens[2].Length, Operators);
         var variableName = tokens[3];
         Color(variableName.Length, Variables);
         Replacement = new AtReplacement(variableName, tokens[1].Trim() == "@");
         return new NullOp();
      }

      public override string VerboseName => "at replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}