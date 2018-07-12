using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class IterateParser : Parser
   {
      FieldListParser parser;
      FreeParser freeParser;

      public IterateParser()
         : base("^ /(|tabs| 'iterate') /b")
      {
         parser = new FieldListParser();
         freeParser = new FreeParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         if (parser.Parse(source, NextPosition).If(out var fields, out var index))
         {
            var parameters = new Parameters(fields);
            if (freeParser.Scan(source, index, "^ /s* 'in' /b"))
            {
               index = freeParser.Position;
               freeParser.ColorAll(KeyWords);
               if (GetExpression(source, index, EndOfLineConsuming()).If(out var expression, out var i))
               {
                  index = i;
                  var block = GetBlock(source, index, true);
                  if (block.IsSome)
                  {
                     index = block.Value.Item2;

                     var firstParser = new FirstParser();
                     var first = when(firstParser.Scan(source, index), () => firstParser.Block);
                     if (first.IsSome)
                        index = firstParser.Position;

                     var middleParser = new MiddleParser();
                     var middle = when(middleParser.Scan(source, index), () => middleParser.Block);
                     if (middle.IsSome)
                        index = middleParser.Position;
                     else
                        return null;

                     var lastParser = new LastParser();
                     var last = when(lastParser.Scan(source, index), () => lastParser.Block);
                     if (last.IsSome)
                        index = lastParser.Position;

                     overridePosition = index;
                     return new Iterate(parameters, expression, first, middle.Value, last, block.Value.Item1) { Index = position };
                  }
               }
            }
         }

         return null;
      }

      public override string VerboseName => "iterate";
   }
}