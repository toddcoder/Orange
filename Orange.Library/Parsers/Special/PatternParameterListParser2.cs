using System.Collections.Generic;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ComparisandParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Values.Parameter;
using static Standard.Types.Maybe.MaybeFunctions;
using Parameter = Orange.Library.Values.Parameter;

namespace Orange.Library.Parsers.Special
{
   public class PatternParameterListParser2 : SpecialParser<List<Parameter>>, IReturnsParameterList
   {
      List<Parameter> list;

      public PatternParameterListParser2() => list = new List<Parameter>();

      public override IMaybe<(List<Parameter>, int)> Parse(string source, int index)
      {
         if (freeParser.Scan(source, index, "^ /s* ']'"))
            return none<(List<Parameter>, int)>();

         var bindingIndex = 0;
         while (index < source.Length)
         {
            var comparisand = GetComparisand(source, index, PassAlong("^ /s* [',]']", false));
            if (comparisand.If(out var comparisandExpression, out var condition, out var newIndex))
            {
               index = newIndex;
               var parameter = FromComparisand(bindingIndex++, comparisandExpression, condition);
               list.Add(parameter);
               if (freeParser.Scan(source, index, "^ /s* ','"))
               {
                  freeParser.ColorAll(Structures);
                  index = freeParser.Position;
                  continue;
               }

               if (freeParser.Scan(source, index, "^ /s* ']'"))
               {
                  freeParser.ColorAll(Structures);
                  return (list, freeParser.Position).Some();
               }
            }
         }

         return none<(List<Parameter>, int)>();
      }

      public bool Multi { get; set; } = true;

      public bool Currying { get; set; }
   }
}