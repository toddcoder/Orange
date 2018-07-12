using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Replacements
{
   public class ValueReplacementParser : Parser, IReplacementParser
   {
      public ValueReplacementParser()
         : base($"^ /(/s* '#') /('@')? /({REGEX_VARIABLE})", true)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Operators);
         var array = tokens[2];
         Color(array.Length, Operators);
         var variableName = tokens[3];
         Color(variableName.Length, Variables);
         Replacement = array.Length > 0 ? (IReplacement)new ValueArrayReplacement(variableName) :
            new ValueReplacement(variableName);
         return new NullOp();
      }

      public override string VerboseName => "value replacement";

      public IReplacement Replacement
      {
         get;
         set;
      }
   }
}