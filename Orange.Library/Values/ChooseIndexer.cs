using System.Linq;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class ChooseIndexer : Variable
   {
      Array array;
      Block keyBlock;

      public ChooseIndexer(Array array, Block keyBlock)
      {
         this.array = array;
         this.keyBlock = keyBlock;
      }

      public override int Compare(Value value) => 0;

      public override ValueType Type
      {
         get;
      } = ValueType.ChooseIndexer;

      public override bool IsTrue
      {
         get;
      } = false;

      public override Value Clone() => new ChooseIndexer((Array)array.Clone(), (Block)keyBlock.Clone());

      protected override void registerMessages(MessageManager manager)
      {
      }

      static bool keysAllInt(Value[] values)
      {
         return values.All(v => v.Type == ValueType.Number && v.Number == (int)v.Number);
      }

      Variable getIndexer()
      {
         var keys = (Array)keyBlock.ToActualArguments().Flatten();
         if (keysAllInt(keys.Values))
            return new IndexIndexer(array, keys);
         return new KeyIndexer(array, keys);
      }

      public override Value Value
      {
         get => getIndexer().Value;
         set => getIndexer().Value = value;
      }

      public override Value AlternateValue(string message) => getIndexer();
   }
}