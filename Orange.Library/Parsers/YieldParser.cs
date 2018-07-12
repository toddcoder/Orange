using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;

namespace Orange.Library.Parsers
{
   public class YieldParser : Parser
   {
      public YieldParser()
         : base("^ /(|tabs| 'yield') /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         if (GetExpression(source, NextPosition, Stop.EndOfLine()).If(out var expression, out var index))
         {
            overridePosition = index;
            return new Yield(expression) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "yield";
   }
}