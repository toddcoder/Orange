using Orange.Library.Values;

namespace Orange.Library
{
   public interface INSGeneratorSource
   {
      INSGenerator GetGenerator();

      Value Next(int index);

      bool IsGeneratorAvailable
      {
         get;
      }

      Array ToArray();
   }
}