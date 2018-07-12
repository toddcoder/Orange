using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class ArbElementParser : Parser, IElementParser
   {
      public ArbElementParser()
         : base("^ /s* /('*'1%2)")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         Element = tokens[1] == "**" ? (Element)new MArbElement() : new ArbElement();
         return new NullOp();
      }

      public override string VerboseName => "arb element";

      public Element Element
      {
         get;
         set;
      }
   }
}