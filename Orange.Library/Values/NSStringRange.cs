using System;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Strings;
using static System.StringComparison;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Standard.Types.Lambdas.LambdaFunctions;

namespace Orange.Library.Values
{
   public class NSStringRange : Value, INSGeneratorSource
   {
      protected string start;
      protected string stop;
      protected bool forward;
      protected bool inclusive;
      protected Func<bool> compare;
      protected string current;

      public NSStringRange(string start, string stop, bool inclusive)
      {
         this.start = start;
         this.stop = stop;
         forward = string.Compare(start, stop, Ordinal) < 0;
         this.inclusive = inclusive;
         current = "";
         setCompare();
      }

      void setCompare()
      {
         if (inclusive)
            compare = forward ? func(() => string.Compare(current, stop, Ordinal) <= 0) :
               func(() => string.Compare(current, stop, Ordinal) >= 0);
         else
            compare = forward ? func(() => string.Compare(current, stop, Ordinal) < 0) :
               func(() => string.Compare(current, stop, Ordinal) > 0);
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => true;

      public override Value Clone() => new NSStringRange(start, stop, inclusive);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "array", v => ((NSStringRange)v).ToArray());
         manager.RegisterMessage(this, "in", v => ((NSStringRange)v).In());
         manager.RegisterMessage(this, "notIn", v => ((NSStringRange)v).NotIn());
         manager.RegisterProperty(this, "min", v => ((NSStringRange)v).Min());
         manager.RegisterProperty(this, "max", v => ((NSStringRange)v).Max());
      }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      string succ() => current.Length == 1 ? ((char)(current[0] + 1)).ToString() : current.Succ();

      string pred() => current.Length == 1 ? ((char)(current[0] - 1)).ToString() : current.Pred();

      public Value Next(int index)
      {
         if (index == 0)
            current = start;
         else if (forward)
            current = succ();
         else
            current = pred();
         return compare() ? (Value)current : NilValue;
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public override string ToString() => $"'{start}'{(inclusive ? ".." : "...")}'{stop}'";

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

      public override bool IsArray => true;

      public override Value SourceArray => ToArray();

      public Value In()
      {
         var needle = Arguments[0];
         var iterator = new NSIterator(GetGenerator());
         return iterator.Any(value => needle.Compare(value) == 0);
      }

      public Value NotIn() => !In().IsTrue;

      public Value Min() => start.CompareTo(stop) < 0 ? start : stop;

      public Value Max() => stop.CompareTo(start) > 0 ? stop : start;
   }
}