using Orange.Library.Values;
using Standard.Types.Maybe;
using Standard.Types.Objects;
using Standard.Types.Strings;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class Range : Verb, IWrapping, IInsideIndexer
   {
      const string LOCATION = "Range";

      public static ISequenceSource GetSequenceSource(Value left, Value right)
      {
         if (right.Type == ValueType.Alternation)
            return new AlternationStream((Alternation)right);
         if (left.Type == ValueType.ArrayStream)
         {
            var stream = (ArrayStream)left;
            if (right.IsExecutable)
               return stream.CFor(new ParameterBlock(right));
            stream.Limit = (int)right.Number;
            return stream;
         }
         if (!left.IsExecutable && right.IsExecutable)
            return new ArrayStream(left, new ParameterBlock(right));
         return null;
      }

      public static IRange GetRange(Value left, Value right, IMaybe<int> length, bool inside = false)
      {
         var range = left.As<IRange>();
         if (range.IsSome)
         {
            range.Value.SetStop(right);
            return range.Value;
         }

         var dateRange = left.As<DateRange>();
         if (dateRange.IsSome)
         {
            dateRange.Value.SetStop(right.Text);
            return dateRange.Value;
         }

         if (left.Type == ValueType.Number && right.Type == ValueType.Number)
         {
            var start = (int)left.Number;
            var stop = (int)right.Number;
            return new IntRange(start, stop, length)
            {
               Inside = inside
            };
         }

         if (left.Type == ValueType.String)
         {
            var start = left.Text;
            var stop = right.Text;
            Reject(start.IsEmpty(), LOCATION, "Start can't be an empty string");
            Reject(stop.IsEmpty(), LOCATION, "Stop can't be an empty string");
            return new StringRange(start, stop);
         }

         if (left.Type == ValueType.Block && right.Type == ValueType.Block)
         {
            var start = (Block)left;
            var stop = (Block)right;
            return new BlockRange(start, stop);
         }

         if (left.Type == ValueType.Date && right.Type == ValueType.Date)
         {
            var start = (Date)left;
            var stop = (Date)right;
            return new DateRange(start, stop);
         }

         if (left.Type == ValueType.Rational)
         {
            var start = (Rational)left;
            var stop = Rational.Cast(right);
            return new RationalRange(start, stop);
         }

         if (left.Type == ValueType.Object && right.Type == ValueType.Object)
         {
            var start = (Object)left;
            var stop = (Object)right;
            return new ObjectRange(start, stop);
         }

         return null;
      }

      public static Value GetGenerator(Value left, Value right, bool inclusive)
      {
         if (left.Type == ValueType.Number && right.Type == ValueType.Number)
            return new NSIntRange(left.Int, right.Int, inclusive);
         if (left.Type == ValueType.String && right.Type == ValueType.String)
            return new NSStringRange(left.Text, right.Text, inclusive);
         Throw(LOCATION,"Not a range");
         return null;
      }

      IMaybe<int> length;

      public Range()
      {
         length = new None<int>();
         IsSlice = false;
      }

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var right = stack.Pop(true, LOCATION);
         var left = stack.Pop(true, LOCATION);
         return GetGenerator(left, right, true);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

      public override string ToString() => "to";

      public void SetLength(int newLength) => length = newLength.Some();

      public bool IsSlice
      {
         get;
         set;
      }

      public bool Inside
      {
         get;
         set;
      }
   }
}