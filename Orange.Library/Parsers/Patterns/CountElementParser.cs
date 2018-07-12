using Orange.Library.Patterns;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class CountElementParser : Parser, IElementParser
   {
      public CountElementParser()
         : base("^ /(/s*) /(/d+) " + REGEX_BEGIN_PATTERN)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var countSource = tokens[2];
         Color(countSource.Length, Numbers);

         var count = countSource.ToInt();

         var parser = new PatternParser
         {
            IgnoreReplacement = true
         };
         if (parser.Scan(source, NextPosition - 1))
         {
            var newPattern = (Pattern)parser.Result.Value;
            Element = new CountElement(count, newPattern);
            overridePosition = parser.Result.Position;
            return new NullOp();
         }
         return null;
      }

      public override string VerboseName => "count element";

      public Element Element
      {
         get;
         set;
      }
   }
}