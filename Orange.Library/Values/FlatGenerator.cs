using Standard.Types.DataStructures;
using Standard.Types.Maybe;
using static Orange.Library.Values.Nil;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class FlatGenerator : NSGenerator
   {
      INSGenerator sourceGenerator;
      MaybeQueue<NSIterator> iterators;
      IMaybe<NSIterator> currentIterator;

      public FlatGenerator(INSGenerator sourceGenerator)
         : base(null)
      {
         this.sourceGenerator = sourceGenerator;
         iterators = new MaybeQueue<NSIterator>();
         currentIterator = none<NSIterator>();
      }

      public void Add(Value value)
      {
         var iterator = new NSIterator(value.PossibleGenerator().FlatMap(g => g, () => new NSOneItemGenerator(value)));
         iterators.Enqueue(iterator);
      }

      public override void Reset()
      {
         var sourceIterator = new NSIterator(sourceGenerator);
         foreach (var item in sourceIterator)
            Add(item);
      }

      public override Value Next()
      {
         if (currentIterator.IsSome)
         {
            var value = currentIterator.Value.Next();
            if (value.IsNil)
               currentIterator = none<NSIterator>();
            else
               return value;
         }

         while (iterators.Count > 0)
         {
            currentIterator = iterators.Dequeue();
            if (currentIterator.IsNone)
               return NilValue;

            currentIterator.Value.Reset();

            var value = currentIterator.Value.Next();
            if (!value.IsNil)
               return value;
         }

         return NilValue;
      }

      public override string ToString() => sourceGenerator.ToString();
   }
}