using System.Collections.Generic;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class ParameterListParser2 : SpecialParser<List<Parameter>>, IReturnsParameterList
   {
      ParameterParser parameterParser;

      public ParameterListParser2() => parameterParser = new ParameterParser();

      public string Pattern { get; set; } = "^ /s* /([',;)'])";

      public string EndOfParameter { get; set; } = "^ ')'";

      public override IMaybe<(List<Parameter>, int)> Parse(string source, int index)
      {
         var list = new List<Parameter>();
         while (index < source.Length)
         {
            if (parameterParser.Parse(source, index).If(out var parameter, out var newIndex))
            {
               index = newIndex;
               list.Add(parameter);
            }
            if (freeParser.Scan(source, index, Pattern))
            {
               index = freeParser.Position;
               freeParser.ColorAll(Structures);
               var structure = freeParser.Tokens[1];
               if (structure.IsMatch(EndOfParameter))
                  return (list, index).Some();

               if (structure == ";")
                  Currying = true;
               continue;
            }

            return none<(List<Parameter>, int)>();
         }

         return none<(List<Parameter>, int)>();
      }

      public bool Multi { get; set; }

      public bool Currying { get; set; }
   }
}