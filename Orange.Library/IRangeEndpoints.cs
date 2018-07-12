namespace Orange.Library
{
   public interface IRangeEndpoints
   {
      int Start(int length);

      int Stop(int length);

      int Increment(int length);

      bool Inclusive
      {
         get;
      }
   }
}