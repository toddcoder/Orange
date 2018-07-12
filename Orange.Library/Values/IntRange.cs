using System;
using Orange.Library.Generators;
using Orange.Library.Managers;
using Standard.Types.Maybe;
using static System.Math;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class IntRange : Value, IRange, IInsideIndexer, IGetGenerator, INSGeneratorSource
   {
      public class IntRangeGenerator : NSGenerator
      {
         int start;
         int stop;
         int increment;
         Predicate<int> moreData;

         public IntRangeGenerator(IntRange range)
            : base(range)
         {
            start = range.start;
            stop = range.stop;
            increment = range.increment;
            if (start > stop)
            {
               increment = -Abs(increment);
               moreData = i => i >= stop;
            }
            else
               moreData = i => i <= stop;
         }

         public override void Reset()
         {
            index = start;
         }

         public override Value Next()
         {
            if (moreData(start))
            {
               var current = start;
               start += increment;
               return current;
            }

            return NilValue;
         }
      }

      protected int start;
      protected int stop;
      protected int increment;
      protected IMaybe<int> arrayLength;

      public IntRange(int start, int stop, IMaybe<int> arrayLength)
      {
         this.start = start;
         this.stop = stop;
         increment = start <= stop ? 1 : -1;
         this.arrayLength = arrayLength;
      }

      public IntRange()
         : this(0, 0, new None<int>()) { }

      public Value Start => start;

      public Value Stop => stop;

      public Value Increment
      {
         get { return increment; }
         set { increment = (int)value.Number; }
      }

      public IMaybe<int> ArrayLength
      {
         get { return arrayLength; }
         set { arrayLength = value; }
      }

      public virtual void SetStart(Value nStart) => start = (int)nStart.Number;

      public void SetStop(Value nStop) => stop = (int)nStop.Number;

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return AlternateValue("").Text; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => false;

      public override Value Clone() => new IntRange(start, stop, arrayLength)
      {
         Increment = increment
      };

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "iter", v => ((IntRange)v).Iter());
         manager.RegisterMessage(this, "limit", v => ((IntRange)v).Limit());
         manager.RegisterMessage(this, "start", v => ((IntRange)v).Start);
         manager.RegisterMessage(this, "stop", v => ((IntRange)v).Stop);
         manager.RegisterMessage(this, "inc", v => ((IntRange)v).Increment);
         manager.RegisterMessage(this, "rand", v => ((IntRange)v).Random());
      }

      public Value Random()
      {
         var array = AlternateValue("rand");
         return MessagingState.SendMessage(array, "sample", new Arguments());
      }

      public Value Limit() => new RangeRepeater(this, (int)Arguments[0].Number);

      public Value Iter() => new IntIterator(start, stop)
      {
         Increment = increment
      };

      public override Value AlternateValue(string message)
      {
         if (stop == -1)
            return new Array();

         var length = arrayLength.Map(l => l, () => (int)Ceiling((decimal)((stop - start) / increment)));
         var array = length <= MAX_LOOP ? Array.SliceRange(start, stop, length, Inside, increment) :
            new IntLazyArray(start, stop, increment);
         return array;
      }

      public Value Slice()
      {
         if (stop == -1)
            return new Array();

         var length = arrayLength.Map(l => l, () => (int)Ceiling((decimal)((stop - start) / increment)));
         var array = length <= MAX_LOOP ? Array.SliceRange(start, stop, length, Inside, increment) :
            new IntLazyArray(start, stop, increment);
         return array;
      }

      public override void AssignTo(Variable variable) => AlternateValue("").AssignTo(variable);

      public override string ToString() => $"{start} to {stop}";

      public IGenerator GetGenerator()
      {
         return new RangeGenerator(start, current =>
         {
            var next = (int)current.Number + increment;
            if (increment > 0)
               return next <= stop ? (Value)next : new Nil();

            return next >= stop ? (Value)next : new Nil();
         });
      }

      public Value Next(int index) => null;

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public override Value AssignmentValue() => AlternateValue("assignment value");

      public override bool IsArray => true;

      public override Value SourceArray => AlternateValue("Source Array");

      public override Value SliceArray => Slice();

      public bool Inside { get; set; }

      public override Value ArgumentValue() => AlternateValue("arg");

      INSGenerator INSGeneratorSource.GetGenerator() => new IntRangeGenerator(this);
   }
}