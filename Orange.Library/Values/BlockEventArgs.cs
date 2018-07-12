using System;

namespace Orange.Library.Values
{
   public class BlockEventArgs : EventArgs
   {
      public BlockEventArgs(int index, string result)
      {
         Index = index;
         Result = result;
      }

      public int Index { get; set; }

      public string Result { get; set; }
   }
}