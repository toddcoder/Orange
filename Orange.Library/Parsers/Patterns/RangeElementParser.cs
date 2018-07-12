using Orange.Library.Patterns;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class RangeElementParser : Parser, IElementParser
   {
      public RangeElementParser()
         : base($"^ /(/s*) /(/d+) ':' /(/d+) {REGEX_BEGIN_PATTERN}")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var fromSource = tokens[2];
         Color(fromSource.Length, Numbers);
         Color(1, Structures);
         var toSource = tokens[3];
         Color(toSource.Length, Numbers);

         var from = fromSource.ToInt();
         var to = toSource.ToInt();

         var parser = new PatternParser
         {
            IgnoreReplacement = true
         };
         if (parser.Scan(source, position + length - 1))
         {
            var newPattern = (Pattern)parser.Result.Value;
            Element = new RangeElement(from, to, newPattern);
            overridePosition = parser.Result.Position;
            return new NullOp();
         }
         return null;
      }

      public override string VerboseName => "range element";

      public Element Element
      {
         get;
         set;
      }
   }
}