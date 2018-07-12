namespace Orange.Library.Verbs
{
   public interface IStatement
   {
      string Result { get; }

      string TypeName { get; }

      int Index { get; set; }
   }
}