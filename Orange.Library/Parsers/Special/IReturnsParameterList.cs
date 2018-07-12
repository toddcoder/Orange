namespace Orange.Library.Parsers.Special
{
   public interface IReturnsParameterList
   {
      bool Multi
      {
         get;
         set;
      }

      bool Currying
      {
         get;
         set;
      }
   }
}