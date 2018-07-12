using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class PushIndexerRange : Verb, ISetStop, IWrapping
   {
      const string LOCATION = "Push indexer range";

      IMaybe<IndexerRange> range;
      IMaybe<int> length;

      public PushIndexerRange()
      {
         range = new None<IndexerRange>();
         length = new None<int>();
      }

      public override Value Evaluate()
      {
         var start = (int)State.Stack.Pop(true, LOCATION).Number;
         if (range.IsNone)
            range = new IndexerRange(start).Some();
         if (length.IsSome)
            ((IWrapping)range.Value).SetLength(length.Value);
         return range.Value;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

      public void SetStop(int stop)
      {
         if (range.IsNone)
            range = new IndexerRange(0).Some();
         range.Value.SetStop(stop);
      }

      public void SetLength(int iLength) => length = iLength.Some();

      public bool IsSlice { get; set; } = false;
   }
}