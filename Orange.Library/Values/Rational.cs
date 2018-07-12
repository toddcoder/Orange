using Orange.Library.Managers;
using static System.Math;

namespace Orange.Library.Values
{
   public class Rational : Value
   {
      public static Rational Cast(Value value) => value is Rational r ? r : new Rational(value);

      int numerator;
      int denominator;

      public Rational(int numerator, int denominator) => setValues(numerator, denominator);

      public Rational(Value value)
      {
         if (value is Rational rational)
            setValues(rational.numerator, rational.numerator);
         else
            setValues((int)value.Number, 1);
      }

      void setValues(int anNumerator, int aDenominator)
      {
         var g = gcd(Abs(anNumerator), Abs(aDenominator));
         numerator = anNumerator / g;
         denominator = aDenominator / g;
         var sign = Sign(numerator) / Sign(denominator);
         numerator = sign * Abs(numerator);
         denominator = Abs(denominator);
      }

      static int gcd(int a, int b) => b == 0 ? a : gcd(b, a % b);

      public override int Compare(Value value) =>
         value is Rational rational ? (numerator * rational.denominator).CompareTo(rational.numerator * denominator) :
            AlternateValue("cmp").Compare(value);

      public override string Text
      {
         get => $"({numerator} / {denominator})";
         set { }
      }

      public override double Number
      {
         get => numerator / (double)denominator;
         set { }
      }

      public override ValueType Type => ValueType.Rational;

      public override bool IsTrue => (int)Number != 0;

      public override Value Clone() => new Rational(numerator, denominator);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "add", v => ((Rational)v).Add());
         manager.RegisterMessage(this, "sub", v => ((Rational)v).Sub());
         manager.RegisterMessage(this, "mult", v => ((Rational)v).Mult());
         manager.RegisterMessage(this, "div", v => ((Rational)v).Div());
         manager.RegisterMessage(this, "numer", v => ((Rational)v).Numer());
         manager.RegisterMessage(this, "denom", v => ((Rational)v).Denom());
         manager.RegisterMessage(this, "num", v => v.Number);
         manager.RegisterMessage(this, "fmt", v => ((Rational)v).Format());
      }

      public Value Add()
      {
         var other = Cast(Arguments[0]);
         return new Rational(numerator * other.denominator + other.numerator * denominator, denominator * other.denominator);
      }

      public Value Sub()
      {
         var other = Cast(Arguments[0]);
         return new Rational(numerator * other.denominator - other.numerator * denominator, denominator * other.denominator);
      }

      public Value Mult()
      {
         var other = Cast(Arguments[0]);
         return new Rational(numerator * other.numerator, denominator * other.denominator);
      }

      public Value Div()
      {
         var other = Cast(Arguments[0]);
         return new Rational(numerator * other.denominator, denominator * other.numerator);
      }

      public Value Numer() => numerator;

      public Value Denom() => denominator;

      public override Value AlternateValue(string message) => Number;

      public override string ToString() => Text;

      public Rational Successor(Rational increment) => new Rational(numerator * increment.denominator +
         increment.numerator * denominator, denominator * increment.denominator);

      public Rational Predecessor(Rational increment) => new Rational(numerator * increment.denominator -
         increment.numerator * denominator, denominator * increment.denominator);

      public Value Format()
      {
         var format = Arguments[0].Text;
         var x = numerator.ToString(format);
         var y = denominator.ToString(format);
         return $"({x} / {y})";
      }
   }
}