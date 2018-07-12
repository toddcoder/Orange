using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Enumerations
{
   public class PlainEnumerationNameParser : Parser, IEnumerationParser
   {
      public PlainEnumerationNameParser()
         : base($"^ /s* /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Variables);
         Builder.Add(tokens[1]);
         return new NullOp();
      }

      public override string VerboseName => "plain enumeration name parser";

      public EnumerationBuilder Builder
      {
         get;
         set;
      }
   }
}