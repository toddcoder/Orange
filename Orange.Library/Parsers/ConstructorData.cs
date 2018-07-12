namespace Orange.Library.Parsers
{
   public class ConstructorData
   {
      public ConstructorData(string name, int count, bool trailing)
      {
         Name = name;
         Count = count;
         Trailing = trailing;
      }

      public string Name { get; }

      public int Count { get; }

      public bool Trailing { get; }
   }
}