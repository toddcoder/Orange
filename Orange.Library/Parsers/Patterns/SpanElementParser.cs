using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Patterns
{
   public class SpanElementParser : Parser, IElementParser
   {
      public SpanElementParser()
         : base("/(^ /s*) /(['+-']1%2) /(['($' quote])") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var modeSource = tokens[2];
         var modeLength = modeSource.Length;
         var mode = modeLength == 1;
         var not = modeSource.StartsWith("-");
         var type = tokens[3];

         var spaceLength = tokens[1].Length;
         Color(position, spaceLength, Whitespaces);
         Color(modeLength, Operators);
         var index = spaceLength + modeLength;

         switch (type)
         {
            case "(":
               Color(1, Structures);
               var initialPosition = position + length;
               if (GetExpression(source, initialPosition, CloseParenthesis()).If(out var text, out initialPosition))
               {
                  if (not)
                     Element = mode ? (Element)new BreakBlockElement(text) : new MBreakBlockElement(text);
                  else
                     Element = mode ? (Element)new SpanBlockElement(text) : new MSpanBlockElement(text);
                  overridePosition = initialPosition;
               }
               else
                  return null;

               break;
            case "'":
            case "\"":
            case "$":
               var parser = new StringParser();
               if (parser.Scan(source, position + index))
               {
                  var value = parser.Result.Value;
                  var stringText = value.Text;
                  if (not)
                     Element = mode ? new BreakElement(stringText) : new MBreakElement(stringText);
                  else
                     Element = mode ? new SpanElement(stringText) : new MSpanElement(stringText);
               }
               else
                  throw "String open".Throws();

               overridePosition = parser.Result.Position;
               break;
            default:
               return null;
         }

         return new NullOp();
      }

      public override string VerboseName => "span element";

      public Element Element { get; set; }
   }
}