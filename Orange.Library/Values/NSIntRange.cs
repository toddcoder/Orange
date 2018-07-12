using System;
using System.Linq;
using Orange.Library.Managers;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Standard.Types.Lambdas.LambdaFunctions;

namespace Orange.Library.Values
{
   public class NSIntRange : Value, INSGeneratorSource, IRangeEndpoints
   {
      public class NSIntRangeGenerator : NSGenerator
      {
         protected int start;
         protected int increment;

         public NSIntRangeGenerator(INSGeneratorSource generatorSource, int start, int increment)
            : base(generatorSource)
         {
            this.start = start;
            this.increment = increment;
            index = this.start;
         }

         public override void Reset() => index = start;

         public override Value Next() => generatorSource.Next(index++);

         public override string ToString() => $"{start}.+{increment}";
      }

      protected int start;
      protected int stop;
      protected int increment;
      protected bool inclusive;
      protected Func<int, bool> compare;

      public NSIntRange(int start, int stop, bool inclusive)
      {
         this.start = start;
         this.stop = stop;
         this.inclusive = inclusive;
         increment = 1;
         setCompare();
      }

      void setCompare()
      {
         if (inclusive)
            compare = increment > 0 ? func<int, bool>(i => i <= stop) : func<int, bool>(i => i >= stop);
         else
            compare = increment > 0 ? func<int, bool>(i => i < stop) : func<int, bool>(i => i > stop);
      }

      public NSIntRange(int start, int stop, int increment, bool inclusive)
         : this(start, stop, inclusive)
      {
         this.increment = increment;
         setCompare();
      }

      public NSIntRange(NSIntRange otherRange, int increment)
         : this(otherRange.start, otherRange.stop, otherRange.inclusive)
      {
         this.increment = increment;
         setCompare();
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => true;

      public override Value Clone() => new NSIntRange(start, stop, increment, inclusive);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "array", v => ((NSIntRange)v).ToArray());
         manager.RegisterMessage(this, "in", v => ((NSIntRange)v).In());
         manager.RegisterMessage(this, "notIn", v => ((NSIntRange)v).NotIn());
         manager.RegisterProperty(this, "min", v => ((NSIntRange)v).Min());
         manager.RegisterProperty(this, "max", v => ((NSIntRange)v).Max());
      }

      string incrementString()
      {
         if (increment == 1)
            return "";

         if (increment > 0)
            return $".+{increment}";

         return $".{increment}";
      }

      public override string ToString() => $"{start}{(inclusive ? ".." : "...")}{stop}{incrementString()}";

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

      public INSGenerator GetGenerator() => new NSIntRangeGenerator(this, 0, increment);

      public virtual Value Next(int index)
      {
         var value = start + index * increment;
         return compare(value) ? (Value)value : NilValue;
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public override bool IsArray => true;

      public override Value SourceArray => ToArray();

      public int Start(int length) => start;

      public int Stop(int length) => stop;

      public int Increment(int length) => increment;

      public bool Inclusive => inclusive;

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