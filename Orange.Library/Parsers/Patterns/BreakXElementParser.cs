using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Patterns
{
   public class BreakXElementParser : Parser, IElementParser
   {
      public BreakXElementParser()
         : base("^ /(/s* '*') /(['(' quote])") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length - 1, Operators);
         switch (tokens[2])
         {
            case "(":
               Color(1, Structures);
               if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var block, out var index))
               {
                  Element = new BreakXBlockElement(block);
                  overridePosition = index;
                  return new NullOp();
               }

               return null;
            default:
               var parser = new StringParser();
               if (parser.Scan(source, NextPosition - 1))
               {
                  var text = parser.Value.Text;
                  Element = new BreakXElement(text);
                  overridePosition = parser.Position;
                  return new NullOp();
               }

               return null;
         }
      }

      public override string VerboseName => "break x element";

      public Element Element { get; set; }
   }
}