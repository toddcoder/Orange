using Standard.Types.Maybe;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class IndexerRange : IntRange, IWrapping
   {
      public IndexerRange(int start)
         : base(start, start + 1, new None<int>())
      {
         IsSlice = false;
      }

      public IndexerRange()
         : this(0) { }

      public override void SetStart(Value start)
      {
         var intStart = (int)start.Number;
         if (intStart < 0)
            this.start = WrapIndex(intStart, stop + 1, true);
      }

      public void SetLength(int length)
      {
         stop = length - 1;
         if (start < 0)
            start = WrapIndex(start, stop + 1, true);
      }

      public bool IsSlice { get; set; }

      public override Value ArgumentValue() => this;
   }
}