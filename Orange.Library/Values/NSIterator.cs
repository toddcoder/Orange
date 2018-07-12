using System;
using System.Collections;
using System.Collections.Generic;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class NSIterator : NSGenerator, IEnumerable<Value>
   {
      public static (Array, Array) CombineGenerators(Value target, Value source, bool includeTargetStrings, bool includeSourceStrings)
      {
         var targetGenerator = includeTargetStrings ? target.PossibleGenerator() : target.PossibleIndexGenerator();
         var sourceGenerator = includeSourceStrings ? source.PossibleGenerator() : source.PossibleIndexGenerator();

         var targetIterator = targetGenerator.FlatMap(g => new NSIterator(g), () => new NSIterator(new NSOneItemGenerator(target)));
         targetIterator.Reset();
         var sourceIterator = sourceGenerator.FlatMap(g => new NSIterator(g), () => new NSIterator(new NSOneItemGenerator(source)));
         sourceIterator.Reset();
         var targetArray = new Array();
         var pusher = new Array.Pusher();

         var targetValue = targetIterator.Next();
         while (!targetValue.IsNil)
         {
            targetArray.Add(targetValue);
            var sourceValue = sourceIterator.Next();
            if (sourceValue.IsNil)
               break;

            pusher.Add(sourceValue);
            targetValue = targetIterator.Next();
         }

         if (targetValue.IsNil)
         {
            var sourceValue = sourceIterator.Next();
            while (!sourceValue.IsNil)
            {
               pusher.Push(sourceValue);
               sourceValue = sourceIterator.Next();
            }
         }

         return (targetArray, pusher.Array);
      }

      public class NSIteratorEnumerator : IEnumerator<Value>
      {
         protected Value value;
         protected NSIterator iterator;
         protected int index;
         protected Func<Value, int, bool> continuing;

         public NSIteratorEnumerator(NSIterator iterator)
         {
            this.iterator = iterator;
            this.iterator.Reset();
            index = -1;
            continuing = (v, i) => i < MAX_LOOP;
         }

         public NSIteratorEnumerator(NSIterator iterator, Func<Value, int, bool> continuing)
            : this(iterator) => this.continuing = continuing;

         public void Dispose() { }

         public bool MoveNext()
         {
            value = iterator.Next();
            return !value.IsNil && continuing(value, index);
         }

         public void Reset()
         {
            iterator.Reset();
            index = -1;
         }

         public Value Current => value;

         object IEnumerator.Current => Current;
      }

      public static IMaybe<NSIterator> GetIterator(Value value) => value.IfCast<INSGenerator>().Map(g => new NSIterator(g));

      protected INSGenerator mainGenerator;

      public NSIterator(INSGenerator generator)
         : base(null) => mainGenerator = generator;

      public override void Reset()
      {
         mainGenerator.Reset();
         more = true;
      }

      public override Value Next()
      {
         var value = mainGenerator.Next();

         for (var i = 0; i < MAX_LOOP; i++)
         {
            if (value.IsIgnore)
            {
               value = mainGenerator.Next();
               continue;
            }

            break;
         }

         more = !value.IsNil;
         return value;
      }

      public IEnumerator<Value> GetEnumerator() => new NSIteratorEnumerator(this);

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IEnumerator<Value> Enumerator(Func<Value, int, bool> continuing) => new NSIteratorEnumerator(this, continuing);
   }
}