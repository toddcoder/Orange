using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class NSObjectRange : Value, INSGeneratorSource
   {
      public class NSObjectGenerator : NSGenerator
      {
         protected Object start;
         protected Object stop;
         protected IMaybe<Object> current;

         public NSObjectGenerator(Object start, Object stop)
            : base(null)
         {
            this.start = start;
            this.stop = stop;
            current = none<Object>();
            index = 0;
         }

         public override void Reset() => current = start.Some();

         public override Value Next()
         {
            if (current.IsSome && index++ < MAX_LOOP && compare(current.Value, stop) < 0)
            {
               var next = successor(current.Value);
               var result = current.Value;
               current = next.Some();
               return result;
            }

            if (current.IsSome)
            {
               var result = current.Value;
               current = none<Object>();
               return result;
            }

            return NilValue;
         }

         static int compare(Object left, Object right) => SendMessage(left, "cmp", right).Int;

         static Object successor(Object obj) => (Object)SendMessage(obj, "succ");
      }

      Object start;
      Object stop;

      public NSObjectRange(Object start, Object stop)
      {
         this.start = start;
         this.stop = stop;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => true;

      public override Value Clone() => new NSObjectRange(start, stop);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "array", v => ((NSObjectRange)v).ToArray());
         manager.RegisterMessage(this, "in", v => ((NSObjectRange)v).In());
         manager.RegisterMessage(this, "notIn", v => ((NSObjectRange)v).NotIn());
         manager.RegisterProperty(this, "min", v => ((NSObjectRange)v).Min());
         manager.RegisterProperty(this, "max", v => ((NSObjectRange)v).Max());
      }

      public INSGenerator GetGenerator() => new NSObjectGenerator(start, stop);

      public Value Next(int index) => this;

      public bool IsGeneratorAvailable => start.RespondsNoDefault("cmp") && start.RespondsNoDefault("succ");

      public Array ToArray() => GeneratorToArray(this);

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

      public override string ToString() => $"{start}..{stop}";

      public Value Min() => start.Compare(stop) < 0 ? start : stop;

      public Value Max() => stop.Compare(start) > 0 ? stop : start;
   }
}