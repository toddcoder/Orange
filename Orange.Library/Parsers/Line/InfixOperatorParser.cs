using System.Collections.Generic;

namespace Orange.Library.Parsers.Line
{
   public class InfixOperatorParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new ThreeCharacterOperatorParser();
            yield return new TwoCharacterOperatorParser();
            yield return new OneCharacterOperatorParser();
            yield return new DisjointMessageParser();
            yield return new WordOperatorParser();
         }
      }
   }
}