using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class InnerComprehensionParser : Parser
   {
      FieldListParser fieldListParser;
      FreeParser freeParser;

      public InnerComprehensionParser()
         : base($"^ |sp| /({REGEX_VARIABLE})")
      {
         fieldListParser = new FieldListParser();
         freeParser = new FreeParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         if (fieldListParser.Parse(source, position).If(out var fields, out var i))
         {
            var index = i;
            var parameters = new Parameters(fields);
            if (freeParser.Scan(source, index, "^ /s* 'in' /b"))
            {
               index = freeParser.Position;
               freeParser.ColorAll(KeyWords);
               var stop = ComprehensionEnd();
               if (GetExpression(source, index, stop).If(out var generatorSource, out var j))
               {
                  if (!freeParser.Scan(source, j, stop.Pattern))
                     return null;

                  index = freeParser.Position;
                  var token = freeParser.Tokens[1];
                  var ifBlock = none<Block>();
                  switch (token)
                  {
                     case ",":
                        Last = false;
                        freeParser.ColorAll(Structures);
                        break;
                     case "if":
                        freeParser.ColorAll(KeyWords);
                        if (GetExpression(source, index, stop).If(out var block, out var k))
                        {
                           ifBlock = block.Some();
                           index = k;
                           freeParser.Scan(source, index, stop.Pattern);
                           index = freeParser.Position;
                           token = freeParser.Tokens[0].TrimLeft();
                           switch (token)
                           {
                              case ",":
                                 Last = false;
                                 freeParser.ColorAll(Structures);
                                 break;
                              case "if":
                                 return null;
                              case ":":
                                 Last = true;
                                 freeParser.ColorAll(Structures);
                                 break;
                           }
                        }

                        break;
                     case ":":
                        Last = true;
                        freeParser.ColorAll(Structures);
                        break;
                  }

                  overridePosition = index;
                  result.Value = new NSInnerComprehension(parameters, generatorSource, ifBlock);
                  return new NullOp();
               }
            }
         }

         return null;
      }

      public override string VerboseName => "inner comprehension";

      public bool Last { get; set; }
   }
}