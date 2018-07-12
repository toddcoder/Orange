using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class FieldScanElementParser : Parser, IElementParser
   {
      public FieldScanElementParser()
         : base("^ /s* /(/d+)? '//'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length - 1, Numbers);
         Color(1, Operators);
         var count = tokens[1].ToInt(-1);
         Element = count == -1 ? (Element)new FieldElement() : new FieldScanElement(count);
         return new NullOp();
      }

      public override string VerboseName => "field element";

      public Element Element
      {
         get;
         set;
      }
   }
}