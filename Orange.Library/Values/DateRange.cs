using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class DateRange : Value, IRange
   {
      Date start;
      Date stop;
      Interval interval;

      public DateRange(Date start, Date stop)
      {
         this.start = start;
         this.stop = stop;
         interval = "1.00:00:00.0";
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.DateRange;

      public override bool IsTrue => false;

      public override Value Clone() => null;

      protected override void registerMessages(MessageManager manager) { }

      public Value Increment { get; set; }

      public void SetStart(Value start) { }

      public void SetStop(Value stop) { }

      public Value Start { get; set; }

      public Value Stop { get; set; }

      public Interval Interval
      {
         get => interval;
         set => interval = value;
      }

      public override Value AlternateValue(string message) => getArray();

      Value getArray()
      {
         var array = new Array();
         if (start.Compare(stop) < 0)
            for (var date = (Date)start.Clone(); date.Compare(stop) <= 0; date = date.Add(interval))
               array.Add(date);
         else
            for (var date = (Date)start.Clone(); date.Compare(stop) >= 0; date = date.Sub(interval))
               array.Add(date);

         return array;
      }

      public override Value ArgumentValue() => getArray();

      public override Value AssignmentValue() => getArray();

      public override bool IsArray => true;

      public override Value SourceArray => getArray();

      public override string ToString() => $"{start}..{stop}";
   }
}