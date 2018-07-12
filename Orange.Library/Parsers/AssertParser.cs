using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class AssertParser : Parser
   {
      StringParser stringParser;
      InterpolatedStringParser2 interpolatedStringParser;

      public AssertParser()
         : base("^ /(|tabs|) /(('assert' | 'reject') /s+)")
      {
         stringParser = new StringParser();
         interpolatedStringParser = new InterpolatedStringParser2();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);
         var assert = tokens[2].Trim() == "assert";

         if (GetExpression(source, NextPosition, FuncThen()).If(out var expression, out var index))
         {
            String message;
            if (stringParser.Scan(source, index))
            {
               overridePosition = stringParser.Position;
               message = (String)stringParser.Value;
            }
            else if (interpolatedStringParser.Scan(source, index))
            {
               overridePosition = interpolatedStringParser.Position;
               message = (String)interpolatedStringParser.Value;
            }
            else
               return null;

            return new Assert(assert, expression, message) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "Assert";
   }
}