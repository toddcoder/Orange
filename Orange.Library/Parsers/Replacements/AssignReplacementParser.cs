using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Replacements
{
   public class AssignReplacementParser : Parser, IReplacementParser
   {
      public AssignReplacementParser()
         : base($"^ /(/s*) /(['//*'])? /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var local = tokens[2] == "/";
         var bind = tokens[2] == "*";
         var variableName = tokens[3];
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Operators);
         Color(variableName.Length, Variables);
         Replacement = new AssignReplacement(variableName, local, bind);
         return new NullOp();
      }

      public override string VerboseName => "assignment replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}