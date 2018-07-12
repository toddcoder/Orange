using System.Collections.Generic;
using Orange.Library.Parsers.Line;
using Orange.Library.Scanning;

namespace Orange.Library.Parsers.Scanning
{
   public class ScanItemParser : MultiParser, IScanItem
   {
      public override IEnumerable<Parser> Parsers
      {
         get { yield return new MoveParser(); }
      }

      public override bool Continue(Parser parser, string source)
      {
         if (parser is IScanItem scanItem)
         {
            ScanItem = scanItem.ScanItem;
            return true;
         }

         return false;
      }

      public ScanItem ScanItem { get; set; }
   }
}