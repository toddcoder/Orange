using System;
using System.Linq;
using Orange.Library.Managers;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class NSDateRange : Value, INSGeneratorSource
   {
      public class NSDateRangeGenerator : NSGenerator
      {
         protected DateTime start;

         public NSDateRangeGenerator(INSGeneratorSource generatorSource, DateTime start)
            : base(generatorSource) => this.start = start;
      }

      DateTime start;
      DateTime stop;
      bool inclusive;

      public NSDateRange(Date start, Date stop, bool inclusive)
      {
         this.start = start.DateTime;
         this.stop = stop.DateTime;
         this.inclusive = inclusive;
      }

      public NSDateRange(DateTime start, DateTime stop, bool inclusive)
      {
         this.start = start;
         this.stop = stop;
         this.inclusive = inclusive;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => true;

      public override Value Clone() => new NSDateRange(start, stop, inclusive);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "array", v => ((NSDateRange)v).ToArray());
         manager.RegisterMessage(this, "in", v => ((NSDateRange)v).In());
         manager.RegisterMessage(this, "notIn", v => ((NSDateRange)v).NotIn());
         manager.RegisterProperty(this, "min", v => ((NSDateRange)v).Min());
         manager.RegisterProperty(this, "max", v => ((NSDateRange)v).Max());
      }

      public INSGenerator GetGenerator() => new NSDateRangeGenerator(this, start);

      public Value Next(int index)
      {
         var current = start.AddDays(index);
         if (inclusive)
            return current <= stop ? (Value)current : NilValue;

         return current < stop ? (Value)current : NilValue;
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public override string ToString() => $"{start}{(inclusive ? ".." : "...")}{stop}";

      public override Value AlternateValue(string message)
      {
         switch (message)
         {
            case "__$get_item":
            case "__$set_item":
            case "len":
               return ToArray();
            default:
               return (Value)GetGenerator();
         }
      }

      public Value In()
      {
         var needle = Arguments[0];
         var iterator = new NSIterator(GetGenerator());
         return iterator.Any(value => needle.Compare(value) == 0);
      }

      public Value NotIn() => !In().IsTrue;

      public Value Min() => start < stop ? start : stop;

      public Value Max() => stop > start ? stop : start;
   }
}