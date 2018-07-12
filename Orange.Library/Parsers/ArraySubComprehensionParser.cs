using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using If = Orange.Library.Verbs.If;

namespace Orange.Library.Parsers
{
   public class ArraySubComprehensionParser : Parser
   {
      FieldListParser fieldListParser;
      FreeParser freeParser;

      public ArraySubComprehensionParser()
         : base($"^ ' '* /({REGEX_VARIABLE})")
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
            if (freeParser.Scan(source, index, "^ ' '* 'in' /b"))
            {
               index = freeParser.Position;
               freeParser.ColorAll(KeyWords);
               if (GetExpression(source, index, PassAlong("^ ' '* 'do' /b", true, KeyWords)).If(out var arrayExp, out var j))
               {
                  index = j;
                  var newSource = new Block();
                  var ifExpression = new Block();
                  var addingToIf = false;
                  foreach (var verb in arrayExp.AsAdded)
                  {
                     if (verb is If)
                     {
                        addingToIf = true;
                        continue;
                     }

                     if (addingToIf)
                        ifExpression.Add(verb);
                     else
                        newSource.Add(verb);
                  }

                  if (GetExpression(source, index, PassAlong("^ ' '* [',)']", false)).If(out var yieldExp, out var k))
                  {
                     overridePosition = k;
                     var comprehension = new Comprehension(yieldExp, parameters, new Region()) { ArrayBlock = newSource };
                     if (ifExpression.Count > 0)
                        comprehension.SetIf(ifExpression);
                     result.Value = comprehension;
                     return new NullOp();
                  }
               }
            }
         }

         return null;
      }

      public override string VerboseName => "array sub comprehension";
   }
}