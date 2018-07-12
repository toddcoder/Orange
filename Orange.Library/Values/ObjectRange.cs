using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using Standard.Types.Objects;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class ObjectRange : Value, IRange, INSGeneratorSource
   {
      public class ObjectRangeGenerator : NSGenerator
      {
         Object start;
         Object stop;
         Object current;

         public ObjectRangeGenerator(ObjectRange range)
            : base(range)
         {
            start = range.start;
            stop = range.stop;
         }

         public override void Reset()
         {
            current = start;
         }

         public override Value Next()
         {
            if (current.SendToSelf("cmp", stop).Int <= 0)
            {
               var deferred = current;
               current = (Object)current.SendToSelf("succ");
               return deferred;
            }

            return NilValue;
         }
      }

      const string LOCATION = "Object Range";
      const string ERROR_MESSAGE = "Start doesn't support cmp and/or succ messages";

      static bool supportsMessages(Object obj) => obj.RespondsNoDefault("cmp") && obj.RespondsNoDefault("succ");

      Object start;
      Object stop;

      public ObjectRange(Object start, Object stop)
      {
         this.start = start;
         this.stop = stop;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            var array = (Array)AlternateValue("");
            return array.Values.Select(v => SendMessage(v, "str")).Listify(State.FieldSeparator.Text);
         }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => false;

      public override Value Clone() => new ObjectRange((Object)start.Clone(), (Object)stop.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "in", v => ((ObjectRange)v).In());
      }

      public Value In()
      {
         if (Arguments[0] is Object obj)
         {
            Assert(supportsMessages(start), LOCATION, ERROR_MESSAGE);
            Assert(supportsMessages(stop), LOCATION, ERROR_MESSAGE);
            Assert(supportsMessages(obj), LOCATION, ERROR_MESSAGE);
            for (var current = start; compare(current, stop); current = next(current))
               if (compare(current, obj))
                  return true;
         }

         return false;
      }

      public Value Increment { get; set; }

      public void SetStart(Value oStart)
      {
         if (oStart is Object newStart)
            start = newStart;
      }

      public void SetStop(Value oStop)
      {
         if (oStop is Object newStop)
            stop = newStop;
      }

      public Value Start => start;

      public Value Stop => stop;

      public override Value AlternateValue(string message)
      {
         Assert(supportsMessages(start), LOCATION, ERROR_MESSAGE);
         Assert(supportsMessages(stop), LOCATION, ERROR_MESSAGE);
         var array = new Array();
         for (var current = start; compare(current, stop); current = next(current))
            array.Add(current);

         array.Add(stop);
         return array;
      }

      static Object next(Object current) => current.SendToSelf("succ").RequiredCast<Object>(() => "succ message didn't return an object");

      static bool compare(Object left, Object right) => left.SendToSelf("cmp", right, () => -1) == 0;

      public override string ToString() => $"{start} to {stop}";

      public override bool IsArray => true;

      public override Value SourceArray => AlternateValue("array");

      public override Value ArgumentValue() => AlternateValue("arg");

      public INSGenerator GetGenerator() => new ObjectRangeGenerator(this);

      public Value Next(int index) => null;

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);
   }
}