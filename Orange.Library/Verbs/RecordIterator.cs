using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class RecordIterator : Verb
   {
      Block block;
      string recordVariable;
      string recordIndexVariable;
      string recordNumberVariable;

      public RecordIterator(Block block, string recordVariable, string recordIndexVariable, string recordNumberVariable)
      {
         this.block = block;
         this.recordVariable = recordVariable;
         this.recordIndexVariable = recordIndexVariable;
         this.recordNumberVariable = recordNumberVariable;
      }

      public override Value Evaluate()
      {
         var input = Runtime.State.Stack.Pop(true, "").Text;
         var records = Runtime.State.RecordPattern.Split(input);
         for (var i = 0; i < records.Length; i++)
         {
            var record = records[i];
            RegionManager.Regions.SetLocal(recordVariable, record);
            RegionManager.Regions.SetLocal(recordIndexVariable, i);
            RegionManager.Regions.SetLocal(recordNumberVariable, i + 1);
            block.Evaluate();
            records[i] = RegionManager.Regions[recordVariable].Text;
         }
         return null;
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Invoke;
   }
}