using System.Collections.Generic;
using Orange.Library.Parsers.Line;

namespace Orange.Library.Parsers
{
   public class BlockOrLambdaParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new LambdaBlockParser();
            yield return new DoLambdaParser();
            yield return new LambdaParser();
            yield return new ShortLambdaParser("(");
         }
      }
   }
}