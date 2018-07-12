using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class NSIntRangeByLength : NSIntRange
   {
      public static INSGenerator ReplaceGeneratorSource(INSGenerator originalGenerator, int length)
      {
         if (originalGenerator.GeneratorSource is IRangeEndpoints nsIntRange)
            return new NSGenerator(new NSIntRangeByLength(nsIntRange, length));

         return originalGenerator;
      }

      static int wrap(int value, int length) => value < 0 ? length + value : value;

      protected int length;
      protected bool empty;

      public NSIntRangeByLength(int start, int stop, bool inclusive, int length)
         : base(start, stop, inclusive)
      {
         this.start = wrap(this.start, length);
         this.stop = wrap(this.stop, length);
         this.length = length;
      }

      public NSIntRangeByLength(int start, int stop, int increment, bool inclusive, int length)
         : base(start, stop, increment, inclusive)
      {
         this.start = wrap(this.start, length);
         this.stop = wrap(this.stop, length);
         this.length = length;
      }

      public NSIntRangeByLength(NSIntRange otherRange, int increment, int length)
         : base(otherRange, increment)
      {
         start = wrap(start, length);
         stop = wrap(stop, length);
         this.length = length;
      }

      public NSIntRangeByLength(NSIntRange otherRange, int length)
         : base(otherRange, 1)
      {
         start = wrap(start, length);
         stop = wrap(stop, length);
         this.length = length;
      }

      public NSIntRangeByLength(IRangeEndpoints rangeEndpoints, int length)
         : base(0, 0, rangeEndpoints.Inclusive)
      {
         var endPointStart = wrap(rangeEndpoints.Start(length), length);
         var endPointStop = wrap(rangeEndpoints.Stop(length), length);
         if (endPointStop <= endPointStart && !inclusive)
            empty = true;
         else
         {
            start = endPointStart;
            stop = endPointStop;
         }
         this.length = length;
      }

      public override Value Next(int index)
      {
         if (empty)
            return NilValue;
         var value = base.Next(index);
         return value.Int < length ? value : NilValue;
      }
   }
}