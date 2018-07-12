using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Patterns
{
   public class AnyElementParser : Parser, IElementParser
   {
      public AnyElementParser()
         : base("/(^ /s*) '('") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         Color(1, Structures);
         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var textBlock, out var index))
         {
            Element = new AnyBlockElement(textBlock, 1);
            overridePosition = index;
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "any element";

      public Element Element { get; set; }
   }
}