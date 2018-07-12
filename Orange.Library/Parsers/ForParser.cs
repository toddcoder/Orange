using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class ForParser : Parser
   {
      FieldListParser parser;
      FreeParser freeParser;

      public ForParser()
         : base("^ /(|tabs| 'for') /b")
      {
         parser = new FieldListParser();
         freeParser = new FreeParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);

         if (parser.Parse(source, NextPosition) is Some<(string[], int)> some)
         {
            (var fields, var index) = some.Value;
            var parameters = new Parameters(fields);
            if (freeParser.Scan(source, index, "^ /s* 'in' /b"))
            {
               index = freeParser.Position;
               freeParser.ColorAll(KeyWords);
               if (GetExpressionThenBlock(source, index) is Some<(Block, Block, int)> some2)
               {
                  (var expression, var block, var blkIndex) = some2.Value;
                  overridePosition = blkIndex;
                  return new ForExecute(parameters, expression, block) { Index = position };
               }
            }
         }

         return null;
      }

      public override string VerboseName => "for";
   }
}