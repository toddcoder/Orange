using Orange.Library.Values;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Parser;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class ExtraMessageWordParser
   {
      Matcher matcher;
      BlockOrLambdaParser blockParser;

      public ExtraMessageWordParser()
      {
         matcher = new Matcher();
         blockParser = new BlockOrLambdaParser();
      }

      public IMaybe<(int, string, Lambda)> Parse(string source, int index)
      {
         if (matcher.IsMatch(source.Substring(index), @"^ /(':') /('while' | 'until')"))
         {
            Color(index, matcher[0, 1].Length, Structures);
            var word = matcher[0, 2];
            Color(word.Length, Messaging);
            if (blockParser.Scan(source, index + matcher[0].Length))
            {
               var result = blockParser.Result;
               var value = result.Value;
               if (!(value is Lambda lambda))
                  lambda = new Lambda(new Region(), (Block)value, new Parameters(), false);
               index = result.Position;
               word = word.ToTitleCase();
               return (index, word, lambda).Some();
            }
         }

         return none<(int, string, Lambda)>();
      }
   }
}