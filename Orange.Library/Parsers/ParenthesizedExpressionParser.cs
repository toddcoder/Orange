using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class ParenthesizedExpressionParser : Parser
   {
      public ParenthesizedExpressionParser()
         : base("^ ' '* '('") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, IDEColor.EntityType.Structures);
         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var block, out var index))
         {
            overridePosition = index;
            return new Push(block);
         }

         var shortLambdaParser = new ShortLambdaParser("");
         if (shortLambdaParser.Scan(source, NextPosition))
         {
            overridePosition = shortLambdaParser.Position;
            return shortLambdaParser.Verb;
         }

         return null;
      }

      public override string VerboseName => "parenthesized";
   }
}