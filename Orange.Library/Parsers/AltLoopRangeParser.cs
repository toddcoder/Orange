using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using Block = Orange.Library.Values.Block;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class AltLoopRangeParser : Parser
   {
      FreeParser freeParser;

      public AltLoopRangeParser()
         : base($"^ /(|sp|) /'(' /({REGEX_VARIABLE}) /(/s* '<-' /s*)") => freeParser = new FreeParser();

      public override Verb CreateVerb(string[] tokens)
      {
         var variable = tokens[3];
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Structures);
         Color(variable.Length, Variables);
         Color(tokens[4].Length, Structures);

         if (GetExpression(source, NextPosition, Comma()).If(out var init, out var i) &&
            GetExpression(source, i, CommaOrCloseParenthesis()).If(out var condition, out var j))
         {
            var index = j;
            Block increment;
            if (freeParser.Scan(source, index, "^ |sp| ','"))
            {
               freeParser.ColorAll(Structures);
               index = freeParser.Position;
               var pIncrement = GetExpression(source, index, Comma());
               if (!pIncrement.If(out increment, out index))
                  return null;

               if (freeParser.Scan(source, index, "^ |sp| ')'"))
               {
                  freeParser.ColorAll(Structures);
                  index = freeParser.Position;
               }
               else
                  return null;
            }
            else
            {
               if (freeParser.Scan(source, index, "^ |sp| ')'"))
               {
                  freeParser.ColorAll(Structures);
                  index = freeParser.Position;
                  var builder = new CodeBuilder();
                  builder.Variable(variable);
                  builder.Verb(new Add());
                  builder.Value(1);
                  increment = builder.Block;
               }
               else
                  return null;
            }

            overridePosition = index;
            var value = new LoopRange(variable, init, true, condition, increment, none<Block>());
            result.Value = value;
            return value.PushedVerb;
         }

         return null;
      }

      public override string VerboseName => "loop range";
   }
}