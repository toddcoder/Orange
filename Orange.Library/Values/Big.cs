using System;
using System.Numerics;
using Orange.Library.Managers;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Big : Value
   {
      BigInteger value;

      public Big(Value value) => this.value = new BigInteger(value.Number);

      public Big(string representation)
      {
         BigInteger.TryParse(representation, out value);
         RejectNull(value, "Big", $"Couldn't convert {representation}L");
      }

      public Big(BigInteger value) => this.value = value;

      public Big()
         : this(0) { }

      public override int Compare(Value other) => other is Big big ? value.CompareTo(big) : value.CompareTo(other);

      int compareTo(Value other) => value.CompareTo(new BigInteger(other.Number));

      int compareTo(Big big) => value.CompareTo(big.value);

      public override string Text
      {
         get => value.ToString();
         set { }
      }

      public override double Number
      {
         get => (double)value;
         set { }
      }

      public override ValueType Type => ValueType.Big;

      public override bool IsTrue => value != 0;

      public override Value Clone() => new Big(value);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "abs", v => ((Big)v).Abs());
         manager.RegisterMessage(this, "add", v => ((Big)v).Add());
         manager.RegisterMessage(this, "sub", v => ((Big)v).Sub());
         manager.RegisterMessage(this, "div", v => ((Big)v).Div());
         manager.RegisterMessage(this, "divrem", v => ((Big)v).DivRem());
         manager.RegisterMessage(this, "mult", v => ((Big)v).Mult());
         manager.RegisterMessage(this, "neg", v => ((Big)v).Neg());
         manager.RegisterMessage(this, "pow", v => ((Big)v).Pow());
         manager.RegisterMessage(this, "band", v => ((Big)v).BAnd());
         manager.RegisterMessage(this, "bor", v => ((Big)v).BOr());
         manager.RegisterMessage(this, "xor", v => ((Big)v).XOr());
         manager.RegisterMessage(this, "shleft", v => ((Big)v).ShLeft());
         manager.RegisterMessage(this, "shright", v => ((Big)v).ShRight());
         manager.RegisterMessage(this, "mod", v => ((Big)v).Mod());
         manager.RegisterMessage(this, "fact", v => ((Big)v).Factorial());
      }

      static Value operate(Value other, Func<Big, Big> bigFunc, Func<double, Big> otherFunc) =>
         other is Big big ? bigFunc(big) : otherFunc(other.Number);

      public Value Abs() => BigInteger.Abs(value);

      public Value Add() => operate(Arguments[0], b => new Big(value + b.value), d => new Big(value + new BigInteger(d)));

      public Value Sub() => operate(Arguments[0], b => new Big(value - b.value), d => new Big(value - new BigInteger(d)));

      public Value Div() => operate(Arguments[0], b => new Big(value / b.value), d => new Big(value / new BigInteger(d)));

      public Value DivRem() => Arguments[0] is Big big ? DivRem(value, big.value) : DivRem(value, (BigInteger)Arguments[0].Number);

      public static Array DivRem(BigInteger dividend, BigInteger divisor)
      {
         var result = BigInteger.DivRem(dividend, divisor, out var remainder);
         return new Array { result, remainder };
      }

      public Value Mult() => operate(Arguments[0], b => new Big(value * b.value), d => new Big(value * new BigInteger(d)));

      public Value Neg() => BigInteger.Negate(value);

      public Value Pow() => BigInteger.Pow(value, (int)Arguments[0].Number);

      public Value BAnd() => operate(Arguments[0], b => new Big(value & b.value), d => new Big(value & new BigInteger(d)));

      public Value BOr() => operate(Arguments[0], b => new Big(value | b.value), d => new Big(value | new BigInteger(d)));

      public Value XOr() => operate(Arguments[0], b => new Big(value ^ b.value), d => new Big(value ^ new BigInteger(d)));

      public Value ShLeft() => value << (int)Arguments[0].Number;

      public Value ShRight() => value >> (int)Arguments[0].Number;

      public Value Mod() => operate(Arguments[0], b => new Big(value % b.value), d => new Big(value % new BigInteger(d)));

      public override string ToString() => value + "L";

      public override Value AlternateValue(string message) => (double)value;

      public Value Factorial()
      {
         var n = value;
         if (n <= 2)
            return n;

         BigInteger result = 1;
         for (var i = 2; i <= n; i++)
            result *= i;

         return result;
      }
   }
}