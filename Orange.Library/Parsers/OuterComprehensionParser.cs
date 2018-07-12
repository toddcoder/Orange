using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class OuterComprehensionParser : Parser
   {
      InnerComprehensionParser innerComprehensionParser;

      public OuterComprehensionParser()
         : base("^ /(|sp| '(') /'for' /b") => innerComprehensionParser = new InnerComprehensionParser();

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Structures);
         Color(tokens[2].Length, KeyWords);
         var index = NextPosition;
         var head = none<NSInnerComprehension>();
         var current = none<NSInnerComprehension>();

         while (index < source.Length)
         {
            if (innerComprehensionParser.Scan(source, index))
            {
               index = innerComprehensionParser.Position;
               var comprehension = ((NSInnerComprehension)innerComprehensionParser.Value).Some();
               if (head.IsSome)
                  current.Value.NextComprehension = comprehension;
               else
                  head = comprehension;
               current = comprehension;
            }
            else
               return null;

            if (!innerComprehensionParser.Last)
               continue;

            if (head.IsNone)
               return null;

            if (GetExpression(source, index, CloseParenthesis()).If(out var block, out var i))
            {
               overridePosition = i;
               var value = new NSOuterComprehension(head.Value, block);
               result.Value = value;
               return value.PushedVerb;
            }
         }

         return null;
      }

      public override string VerboseName => "outer comprehension";
   }
}