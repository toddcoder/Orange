using System;
using Orange.Library.Managers;
using static System.Numerics.Complex;
using CSComplex = System.Numerics.Complex;

namespace Orange.Library.Values
{
   public class Complex : Value
   {
      CSComplex value;

      public Complex(double real, double imaginary, bool isPolar = false) =>
         value = isPolar ? FromPolarCoordinates(real, imaginary) : new CSComplex(real, imaginary);

      public Complex(Double number) => value = new CSComplex(number.Number, 0);

      public Complex(CSComplex complex) => value = complex;

      public override int Compare(Value value) => this.value.Real.CompareTo(value.Number) * this.value.Imaginary.CompareTo(value.Number);

      public override string Text
      {
         get => ToString();
         set { }
      }

      public override double Number
      {
         get => value.Real;
         set { }
      }

      public override ValueType Type => ValueType.Complex;

      public override bool IsTrue => value.Real != 0 && value.Imaginary != 0;

      public override Value Clone() => new Complex(value);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "abs", v => ((Complex)v).Abs());
         manager.RegisterMessage(this, "acos", v => ((Complex)v).Acos());
         manager.RegisterMessage(this, "add", v => ((Complex)v).Add());
         manager.RegisterMessage(this, "asin", v => ((Complex)v).Asin());
         manager.RegisterMessage(this, "atan", v => ((Complex)v).Atan());
         manager.RegisterMessage(this, "conj", v => ((Complex)v).Conj());
         manager.RegisterMessage(this, "cos", v => ((Complex)v).Cos());
         manager.RegisterMessage(this, "cosh", v => ((Complex)v).Cosh());
         manager.RegisterMessage(this, "exp", v => ((Complex)v).Exp());
         manager.RegisterMessage(this, "log", v => ((Complex)v).Log());
         manager.RegisterMessage(this, "mult", v => ((Complex)v).Mult());
         manager.RegisterMessage(this, "neg", v => ((Complex)v).Neg());
         manager.RegisterMessage(this, "pow", v => ((Complex)v).Pow());
         manager.RegisterMessage(this, "recip", v => ((Complex)v).Recip());
         manager.RegisterMessage(this, "sin", v => ((Complex)v).Sin());
         manager.RegisterMessage(this, "sinh", v => ((Complex)v).Sinh());
         manager.RegisterMessage(this, "sqrt", v => ((Complex)v).Sqrt());
         manager.RegisterMessage(this, "tan", v => ((Complex)v).Tan());
         manager.RegisterMessage(this, "tanh", v => ((Complex)v).Tanh());
         manager.RegisterMessage(this, "sub", v => ((Complex)v).Sub());
         manager.RegisterMessage(this, "div", v => ((Complex)v).Div());
         manager.RegisterMessage(this, "re", v => ((Complex)v).Re());
         manager.RegisterMessage(this, "im", v => ((Complex)v).Im());
      }

      public Value Abs() => CSComplex.Abs(value);

      public Value Acos() => CSComplex.Acos(value);

      static Value operate(Value other, Func<Complex, Complex> complexFunc, Func<double, Complex> otherFunc) =>
         other is Complex c ? complexFunc(c) : otherFunc(other.Number);

      public Value Add() => operate(Arguments[0], c => new Complex(value + c.value), d => new Complex(value + d));

      public Value Asin() => CSComplex.Asin(value);

      public Value Atan() => CSComplex.Atan(value);

      public Value Conj() => Conjugate(value);

      public Value Cos() => CSComplex.Cos(value);

      public Value Cosh() => CSComplex.Cosh(value);

      public Value Exp() => CSComplex.Exp(value);

      public Value Log()
      {
         var baseValue = Arguments[0];
         if (baseValue.IsEmpty)
            return Log10(value);

         var baseNumber = baseValue.Number;
         return CSComplex.Log(value, baseNumber);
      }

      public Value Mult() => operate(Arguments[0], c => new Complex(value * c.value), d => new Complex(value * d));

      public Value Neg() => Negate(value);

      public Value Pow()
      {
         var other = Arguments[0];
         return operate(other, c => new Complex(CSComplex.Pow(value, c.value)), d => new Complex(CSComplex.Pow(value, d)));
      }

      public Value Recip() => Reciprocal(value);

      public Value Sin() => CSComplex.Sin(value);

      public Value Sinh() => CSComplex.Sinh(value);

      public Value Sqrt() => CSComplex.Sqrt(value);

      public Value Tan() => CSComplex.Tan(value);

      public Value Tanh() => CSComplex.Tanh(value);

      public override string ToString() =>
         value.Imaginary < 0 ? $"{value.Real}{value.Imaginary}i" : $"{value.Real}+{value.Imaginary}i";

      public Value Sub() => operate(Arguments[0], c => new Complex(value - c.value), d => new Complex(value - d));

      public Value Div() => operate(Arguments[0], c => new Complex(value / c.value), d => new Complex(value / d));

      public Value Re() => value.Real;

      public Value Im() => value.Imaginary;
   }
}