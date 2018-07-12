using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Unto : Value
   {
      Value start;
      Value stop;
      bool matching;

      public Unto()
      {
         start = new Any();
         stop = new Any();
         matching = false;
      }

      public void SetStartAndStop(Value aStart, Value aStop)
      {
         start = aStart;
         stop = aStop;
      }

      public override int Compare(Value value)
      {
         if (matching)
         {
            if (Case.Match(value, stop, false, null))
               matching = false;
            return 0;
         }

         if (Case.Match(value, start, false, null))
         {
            matching = true;
            return 0;
         }

         return -1;
      }

      public bool CompareTo(Value value)
      {
         if (matching)
         {
            if (Case.Match(value, stop, false, null))
            {
               matching = false;
               return true;
            }
         }
         else if (Case.Match(value, start, false, null))
            matching = true;

         return matching;
      }

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Unto;

      public override bool IsTrue => matching;

      public override Value Clone() => new Unto { start = start.Clone(), stop = stop.Clone() };

      protected override void registerMessages(MessageManager manager) { }

      public override string ToString() => $"{start} unto {stop}";
   }
}