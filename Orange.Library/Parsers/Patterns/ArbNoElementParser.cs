using Orange.Library.Patterns;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class ArbNoElementParser : Parser, IElementParser
   {
      public ArbNoElementParser()
         : base("^ /s* '$'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         var parser = new PatternParser
         {
            IgnoreReplacement = true
         };
         if (parser.Scan(source, position + length))
         {
            var subPattern = (Pattern)parser.Result.Value;
            Element = new ArbNoElement(subPattern);
            overridePosition = parser.Result.Position;
            return new NullOp();
         }
         return null;
      }

      public override string VerboseName => "arb no";

      public Element Element
      {
         get;
         set;
      }
   }
}