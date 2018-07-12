using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Replacements
{
   public class PushReplacementParser : Parser, IReplacementParser
   {
      public PushReplacementParser()
         : base($"^ /(/s*) /(['@+']) /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var variableName = tokens[3];
         Color(position, tokens[1].Length, Whitespaces);
         Color(1, Operators);
         Color(variableName.Length, Variables);
         Replacement = tokens[2] == "@" ? (IReplacement)new PushReplacement(variableName) :
            new UniqueReplacement(variableName);
         return new NullOp();
      }

      public override string VerboseName => "push replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}