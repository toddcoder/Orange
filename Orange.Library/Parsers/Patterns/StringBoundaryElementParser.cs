using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class StringBoundaryElementParser : Parser, IElementParser
   {
      public StringBoundaryElementParser()
         : base("^ /(/s*) /(['<>'])")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Operators);
         Element = tokens[2] == "<" ? (Element)new StringBeginElement() : new StringEndElement();
         return new NullOp();
      }

      public override string VerboseName => "string boundary element";

      public Element Element
      {
         get;
         set;
      }
   }
}