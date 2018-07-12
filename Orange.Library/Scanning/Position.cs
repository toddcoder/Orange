namespace Orange.Library.Scanning
{
   public class Position
   {
      int index;
      int length;

      public Position(int index, int length)
      {
         this.index = index;
         this.length = length;
      }

      public int Index => index;

      public int Length => length;
   }
}