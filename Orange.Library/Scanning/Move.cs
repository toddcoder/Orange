using Orange.Library.Values;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Scanning
{
   public class Move : ScanItem
   {
      Block expression;

      public Move(Block expression) => this.expression = expression;

      public override IMaybe<Position> Scan(string source, Position position)
      {
         var length = (int)expression.Evaluate().Number;
         var newLength = position.Length + length;
         return when(newLength < source.Length, () => new Position(position.Index, newLength));
      }
   }
}