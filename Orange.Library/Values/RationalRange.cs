using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class RationalRange : Value, IRange, IInsideIndexer
   {
      Rational start;
      Rational stop;
      Rational increment;

      public RationalRange(Rational start, Rational stop)
         : this(start, stop, new Rational(1, 1))
      {
      }

      public RationalRange(Rational start, Rational stop, Rational increment)
      {
         this.start = start;
         this.stop = stop;
         this.increment = increment;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            return AlternateValue("str").Text;
         }
         set
         {
         }
      }

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => false;

      public override Value Clone() => new RationalRange((Rational)start.Clone(), (Rational)stop.Clone(), (Rational)increment.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "start", v => ((RationalRange)v).Start);
         manager.RegisterMessage(this, "stop", v => ((RationalRange)v).Stop);
         manager.RegisterMessage(this, "inc", v => ((RationalRange)v).Increment);
      }

      public Value Increment
      {
         get => increment;
         set => increment = Rational.Cast(value);
      }

      public void SetStart(Value start)
      {
         this.start = (Rational)start;
      }

      public void SetStop(Value stop)
      {
         this.stop = (Rational)stop;
      }

      public Value Start => start;

      public Value Stop => stop;

      public bool Inside
      {
         get;
         set;
      }

      public override bool IsArray => true;

      Array getArray()
      {
         var array = new Array();
         if (start.Compare(stop) < 0)
            for (var current = start; current.Compare(stop) <= 0; current = current.Successor(increment))
               array.Add(current);
         else
            for (var current = start; current.Compare(stop) >= 0; current = current.Predecessor(increment))
               array.Add(current);
         return array;
      }

      public override Value AlternateValue(string message) => getArray();

      public override Value SourceArray => getArray();
   }
}