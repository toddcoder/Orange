using System;
using System.Collections;
using System.Collections.Generic;

namespace Orange.Library.Values
{
   public class IntLazyArray : Array
   {
      public class IntLazyArrayEnumator : IEnumerator<IterItem>
      {
         IntLazyArray array;
         int index;

         public IntLazyArrayEnumator(IntLazyArray array)
         {
            this.array = array;
            Reset();
         }

         public void Dispose() { }

         public bool MoveNext() => ++index < array.Length;

         public void Reset() => index = -1;

         public IterItem Current => array.GetArrayItem(index);

         object IEnumerator.Current => Current;
      }

      int start;
      int stop;
      int increment;
      int lastIndex;

      public IntLazyArray(int start, int stop, int increment = 1)
      {
         this.start = start;
         this.stop = stop;
         this.increment = increment;
         if (this.start > this.stop)
            this.increment = -Math.Abs(this.increment);
         lastIndex = -1;
      }

      public override Value this[int index]
      {
         get
         {
            if (index > lastIndex)
            {
               var value = (int)base[lastIndex].Number + increment;
               for (var i = lastIndex; i <= index; i++)
               {
                  Add(value);
                  value += increment;
               }
            }

            return base[index];
         }
         set => base[index] = value;
      }

      public override IterItem GetArrayItem(int index)
      {
         var value = this[index];
         var key = indexes[index];
         return new IterItem { Key = key, Value = value, Index = index };
      }

      public override int Length => (int)Math.Ceiling((decimal)((stop - start) / increment));

      public override IEnumerator<IterItem> GetEnumerator() => new IntLazyArrayEnumator(this);
   }
}