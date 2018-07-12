using Orange.Library.Conditionals;
using Orange.Library.Parsers.Conditionals;
using Orange.Library.Parsers.Replacements;
using Orange.Library.Replacements;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class EndPatternParser : Parser, IReplacementParser
   {
      bool ignoreReplacement;
      ReplacementParser replacementParser;
      ConditionalParser conditionalParser;

      public EndPatternParser(bool ignoreReplacement)
         : base($"^ /s* {REGEX_END_PATTERN}")
      {
         this.ignoreReplacement = ignoreReplacement;
         replacementParser = new ReplacementParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         if (ignoreReplacement)
            return new NullOp();

         var index = NextPosition;
         if (replacementParser.Scan(source, index))
         {
            Replacement = replacementParser.Replacement;
            index = replacementParser.Result.Position;
         }
         conditionalParser = new ConditionalParser();
         if (replacementParser.Scan(source, index))
         {
            Conditional = conditionalParser.Conditional;
            index = conditionalParser.Result.Position;
         }
         overridePosition = index;
         return new NullOp();
      }

      public override string VerboseName => "end of pattern";

      public IReplacement Replacement
      {
         get;
         set;
      }

      public Conditional Conditional
      {
         get;
         set;
      }
   }
}