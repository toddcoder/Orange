using Orange.Library.Managers;
using Standard.Types.Objects;

namespace Orange.Library.Values
{
   public class RangeRepeater : Value
   {
      const string LOCATION = "Range repeater";

      Value range;
      int limit;

      public RangeRepeater(Value range, int limit)
      {
         this.range = range;
         this.limit = limit;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.RangeRepeater;

      public override bool IsTrue => false;

      public override Value Clone() => new RangeRepeater(range.Clone(), limit);

      protected override void registerMessages(MessageManager manager) { }

      public override Value AlternateValue(string message)
      {
         range.RequiredCast<IRange>(() => $"{range} not a range");
         var array = new Array();
         while (array.Length <= limit)
         {
            var result = range.AlternateValue(message);
            if (result.Type == ValueType.Array)
            {
               var newArray = (Array)result;
               foreach (var item in newArray)
                  array.Add(item.Value);
            }
            else
               array.Add(result);
         }
         while (array.Length > limit)
            array.Pop();

         return array;
      }
   }
}