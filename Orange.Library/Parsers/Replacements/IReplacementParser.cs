using Orange.Library.Replacements;

namespace Orange.Library.Parsers.Replacements
{
   public interface IReplacementParser
   {
      IReplacement Replacement
      {
         get;
         set;
      }
   }
}