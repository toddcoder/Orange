using System.Collections.Generic;

namespace Orange.Library.Parsers.Line
{
   public class PrefixOperatorParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new SpecialPrefixParser();
            yield return new ChangeSignParser();
            yield return new PreIncrementDecrementParser();
            yield return new NotParser();
            yield return new DefineParser();
            yield return new DeferredParser();
         }
      }
   }
}