using System.Collections.Generic;
using Orange.Library.Parsers.Line;

namespace Orange.Library.Parsers
{
   public class IncrementsParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new AssignToNewFieldParser();
         }
      }
   }
}