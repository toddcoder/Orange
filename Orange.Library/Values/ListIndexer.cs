using System.Linq;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class ListIndexer : Variable
   {
      List list;
      Block indexesBlock;

      public ListIndexer(List list, Block indexesBlock)
         : base($"{VAR_ANONYMOUS}{CompilerState.ObjectID()}")
      {
         this.list = list;
         this.indexesBlock = indexesBlock;
      }

      public override Value Value
      {
         get
         {
            var result = List.Empty;
            var evaluated = indexesBlock.Evaluate();
            if (evaluated.IsArray)
            {
               var indexArray = (Array)evaluated.SourceArray;
               return indexArray
                  .Select(item => (int)item.Value.Number)
                  .Where(index => index >= 0)
                  .Select(index => list[index])
                  .Where(value => value.IsSome)
                  .Aggregate(result, (current, value) => current.Add(value.Value));
            }

            var intIndex = (int)evaluated.Number;
            return list[intIndex].FlatMap(v => v, () => NilValue);
         }
         set { }
      }

      public override string ContainerType => ValueType.ListIndexer.ToString();
   }
}