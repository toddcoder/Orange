using static Orange.Library.Values.NSIntRangeByLength;

namespace Orange.Library.Values
{
   public class NSIteratorByLength : NSIterator
   {
      protected int length;

      public NSIteratorByLength(INSGenerator generator, int length)
         : base(ReplaceGeneratorSource(generator, length)) => this.length = length;
   }
}