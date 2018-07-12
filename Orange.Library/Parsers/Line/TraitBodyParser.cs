using System.Collections.Generic;

namespace Orange.Library.Parsers.Line
{
   public class TraitBodyParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new FunctionParser();
            yield return new SignatureParser(true);
         }
      }
   }
}