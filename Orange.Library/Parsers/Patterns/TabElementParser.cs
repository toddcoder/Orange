using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Patterns
{
   public class TabElementParser : Parser, IElementParser
   {
      public TabElementParser()
         : base("/(^ /s* ['<>']) /('(' | /d+)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var right = tokens[1].EndsWith("<");
         Color(position, tokens[1].Length, Operators);
         var atSource = tokens[2];
         var literal = atSource != "(";
         if (literal)
         {
            Color(atSource.Length, Numbers);
            var at = atSource.ToInt();
            Element = right ? (Element)new TabRightElement(at) : new TabLeftElement(at);
         }
         else
         {
            Color(1, Structures);
            var index = position + length;
            if (GetExpression(source, index, CloseParenthesis()).If(out var at, out index))
            {
               Element = right ? (Element)new TabRightBlockElement(at) : new TabLeftBlockElement(at);
               overridePosition = index;
            }
            else
               return null;
         }

         return new NullOp();
      }

      public override string VerboseName => "tab element";

      public Element Element { get; set; }
   }
}