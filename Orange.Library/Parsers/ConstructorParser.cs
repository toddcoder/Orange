using Orange.Library.Parsers.Special;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class ConstructorParser : SpecialParser<ConstructorData>
   {
      public override IMaybe<(ConstructorData, int)> Parse(string source, int index)
      {
         if (freeParser.Scan(source, index, $"^ /(/s*) /({REGEX_VARIABLE}) /'('?"))
         {
            var tokens = freeParser.Tokens;
            var name = tokens[2];
            freeParser.Colorize(Whitespaces, Types, Structures);
            index = freeParser.Position;
            var count = 0;
            if (tokens[3].IsNotEmpty())
            {
               index = freeParser.Position;
               while (index < source.Length)
                  if (freeParser.Scan(source, index, $"^ /(/s* {REGEX_VARIABLE} /s*) /[',)']"))
                  {
                     index = freeParser.Position;
                     freeParser.Colorize(Variables, Structures);
                     count++;
                     if (freeParser.Tokens[2] == ")")
                        break;
                  }
                  else
                     return none<(ConstructorData, int)>();
            }

            var trailing = freeParser.Scan(source, index, "^ /(/s* '|')");
            if (trailing)
            {
               freeParser.Colorize(Structures);
               index = freeParser.Position;
            }
            return (new ConstructorData(name, count, trailing), index).Some();
         }

         return none<(ConstructorData, int)>();
      }
   }
}