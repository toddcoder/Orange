using System;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Dates;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static System.Math;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Array;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class Double : Value
   {
      protected const string LOCATION = "Double";
      protected const string STR_WORDS = "one two three four five six seven eight nine ten eleven twelve thirteen " +
         "fourteen fifteen sixteen seventeen eighteen nineteen";

      protected readonly double number;
      protected string[] nums;
      protected string[] tens;

      public Double(double number) => this.number = number;

      public override int Compare(Value value) => value is Double vnumber ? number.CompareTo(vnumber.number) : -1;

      public override string ToString() => Text;

      public override string Text
      {
         get => number.ToString();
         set { }
      }

      public override double Number
      {
         get => number;
         set { }
      }

      public override ValueType Type => ValueType.Number;

      public override bool IsTrue => number != 0;

      public override Value Do(bool repeat)
      {
         var value = State.Stack.Pop(true, LOCATION);
         return value.Do((int)number);
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "add", v => ((Double)v).Add());
         manager.RegisterMessage(this, "sub", v => ((Double)v).Sub());
         manager.RegisterMessage(this, "mult", v => ((Double)v).Mult());
         manager.RegisterMessage(this, "div", v => ((Double)v).Div());
         manager.RegisterMessage(this, "mod", v => ((Double)v).Mod());
         manager.RegisterMessage(this, "pow", v => ((Double)v).Pow());
         manager.RegisterMessage(this, "abs", v => Abs(v.Number));
         manager.RegisterMessage(this, "acos", v => Acos(v.Number));
         manager.RegisterMessage(this, "asin", v => Asin(v.Number));
         manager.RegisterMessage(this, "atan", v => Atan(v.Number));
         manager.RegisterMessage(this, "atan2", v => ((Double)v).Atan2());
         manager.RegisterMessage(this, "ceil", v => Ceiling(v.Number));
         manager.RegisterMessage(this, "cos", v => Cos(v.Number));
         manager.RegisterMessage(this, "cosh", v => Cosh(v.Number));
         manager.RegisterMessage(this, "exp", v => Exp(v.Number));
         manager.RegisterMessage(this, "floor", v => Floor(v.Number));
         manager.RegisterMessage(this, "ln", v => Math.Log(v.Number));
         manager.RegisterMessage(this, "log", v => ((Double)v).Log());
         manager.RegisterMessage(this, "max", v => ((Double)v).Max());
         manager.RegisterMessage(this, "min", v => ((Double)v).Min());
         manager.RegisterMessage(this, "round", v => ((Double)v).Round());
         manager.RegisterMessage(this, "sign", v => Sign(v.Number));
         manager.RegisterMessage(this, "sin", v => Sin(v.Number));
         manager.RegisterMessage(this, "sinh", v => Sinh(v.Number));
         manager.RegisterMessage(this, "sqrt", v => ((Double)v).Sqrt());
         manager.RegisterMessage(this, "sqr", v => ((Double)v).Sqr());
         manager.RegisterMessage(this, "tan", v => Tan(v.Number));
         manager.RegisterMessage(this, "tanh", v => Tanh(v.Number));
         manager.RegisterMessage(this, "int", v => Math.Truncate(v.Number));
         manager.RegisterMessage(this, "neg", v => -v.Number);
         manager.RegisterMessage(this, "rand", v => ((Double)v).Random());
         manager.RegisterMessage(this, "times", v => ((Double)v).Times());
         manager.RegisterMessage(this, "to", v => ((Double)v).To());
         manager.RegisterMessage(this, "over", v => ((Double)v).Over());
         manager.RegisterMessage(this, "range", v => ((Double)v).Range());
         manager.RegisterMessage(this, "chr", v => ((Double)v).Chr());
         manager.RegisterMessage(this, "zfill", v => ((Double)v).ZFill());
         manager.RegisterMessage(this, "rangeWhile", v => ((Double)v).RangeWhile(true));
         manager.RegisterMessage(this, "rangeUntil", v => ((Double)v).RangeWhile(false));
         manager.RegisterMessage(this, "toInf", v => ((Double)v).ToInfArray());
         manager.RegisterMessage(this, "isEven", v => (int)v.Number % 2 == 0);
         manager.RegisterMessage(this, "isOdd", v => (int)v.Number % 2 != 0);
         manager.RegisterMessage(this, "hex", v => ((Double)v).Hex());
         manager.RegisterMessage(this, "bin", v => ((Double)v).Binary());
         manager.RegisterMessage(this, "oct", v => ((Double)v).Octal());
         manager.RegisterMessage(this, "compare", v => ((Double)v).Compare());
         manager.RegisterMessage(this, "time", v => ((Double)v).ToTime());
         manager.RegisterMessage(this, "trunc", v => ((Double)v).Truncate());
         //manager.RegisterMessage(this, "iter", v => ((Double)v).Iterate());
         manager.RegisterMessage(this, "mod2", v => ((Double)v).Mod2());
         manager.RegisterMessage(this, "divrem", v => ((Double)v).DivRem());
         manager.RegisterMessage(this, "succ", v => ((Double)v).Succ());
         manager.RegisterMessage(this, "pred", v => ((Double)v).Pred());
         manager.RegisterMessage(this, "key", v => ((Double)v).Key());
         manager.RegisterMessage(this, "isDiv", v => ((Double)v).IsDivBy());
         manager.RegisterMessage(this, "isCloseTo", v => ((Double)v).CloseTo());
         manager.RegisterMessage(this, "randArray", v => ((Double)v).RandomArray());
         manager.RegisterMessage(this, "concat", v => ((Double)v).Concat());
         manager.RegisterMessage(this, "repeat", v => ((Double)v).Repeat());
         manager.RegisterMessage(this, "fact", v => ((Double)v).Factorial());
         manager.RegisterMessage(this, "isPrime", v => ((Double)v).IsPrime());
         manager.RegisterMessage(this, "words", v => ((Double)v).Words());
         manager.RegisterMessage(this, "frac", v => ((Double)v).Fraction());
         manager.RegisterMessage(this, "seq", v => ((Double)v).Seq());
         manager.RegisterMessage(this, "idiv", v => ((Double)v).Div());
         manager.RegisterMessage(this, "gcd", v => ((Double)v).GCD());
         manager.RegisterMessage(this, "lcm", v => ((Double)v).LCM());
         manager.RegisterMessage(this, "rat", v => ((Double)v).Rat());
         manager.RegisterMessage(this, "values", v => ((Double)v).Values());
         manager.RegisterMessage(this, "list", v => ((Double)v).List());
         manager.RegisterMessage(this, "lazy", v => ((Double)v).Lazy());
         manager.RegisterMessage(this, "big", v => ((Double)v).Big());
      }

      public Value Sqrt()
      {
         if (number < 0)
         {
            var complex = new Complex(number, 0);
            return complex.Sqrt();
         }

         return Math.Sqrt(number);
      }

      public Value Sqr() => number * number;

      public Value Repeat()
      {
         var count = (int)Arguments[0].Number;
         if (count == 0)
            return Text;

         return Text.Repeat(count);
      }

      public Value RandomArray()
      {
         var count = (int)Arguments[0].Number;
         if (count == 0)
            return new Array();

         var asInt = (int)number;
         return asInt == 0 ? getRandomArray(count) : getRandomArray(asInt, count);
      }

      static Array getRandomArray(int size, int count) => new Array(Enumerable.Range(0, count)
         .Select(i => (Value)State.Random(size)));

      static Array getRandomArray(int count) => new Array(Enumerable.Range(0, count).Select(i => (Value)State.Random()));

      public Value IsDivBy() => number % Arguments[0].Number == 0;

      public Value Key() => "0" + number;

      public Value Succ() => number + 1;

      public Value Pred() => number - 1;

      public Value DivRem()
      {
         int remainder;
         var divisor = Arguments[0].Int;
         var result = Math.DivRem((int)number, divisor, out remainder);
         return new OTuple(new Value[] { result, remainder });
      }

      public Value Mod2()
      {
         var sign = Sign(number);
         var absNumber = Abs(number);
         var intPart = Math.Truncate(number);
         var fPart = absNumber - intPart;
         intPart *= sign;
         fPart *= sign;
         return new Array(new Value[] { intPart, fPart });
      }

      public Value Within()
      {
         Message1 = new Message("within", Arguments);
         return this;
      }

      public Value Truncate()
      {
         var placesValue = Arguments[0];
         if (placesValue.IsEmpty)
            return Math.Truncate(number);

         var amount = (int)Math.Pow(10, placesValue.Number);
         return Math.Truncate(number * amount) / amount;
      }

      public Value Log()
      {
         var baseValue = Arguments[0];
         if (baseValue.IsEmpty)
            return Log10(number);

         var baseNumber = baseValue.Number;
         return Math.Log(number, baseNumber);
      }

      public override Value AlternateValue(string message)
      {
         switch (message)
         {
            case "rev":
               return Text;
         }

         return new Array();
      }

      public Value ToTime() => new TimeSpan((long)number).ToLongString(true);

      public Value Compare() => number.CompareTo(Arguments[0].Number);

      public Value Octal() => Convert.ToString((int)number, 8);

      public Value Binary() => Convert.ToString((int)number, 2);

      public Value Hex() => Convert.ToString((int)number, 16);

      public Value ToInfArray()
      {
         var varName = Arguments.VariableName(0, VAR_VALUE);
         var block = Arguments.Executable;
         return block.CanExecute ? (Value)new InfArray(varName, block) : new Nil();
      }

      public Value RangeWhile(bool isWhile)
      {
         var varName = Arguments.VariableName(0, VAR_VALUE);
         var block = Arguments.Executable;
         var array = new Array();
         if (block.CanExecute)
         {
            Regions.Push("range-while");
            for (var i = (int)number; i < MAX_LOOP; i++)
            {
               Regions.SetLocal(varName, i);
               if (block.IsTrue || !isWhile)
                  array.Add(i);
               else
               {
                  Regions.Pop("range-while");
                  return array;
               }
            }

            Regions.Pop("range-while");
         }

         return array;
      }

      public Value ZFill()
      {
         var amount = Arguments[0].Int;
         var victim = number;
         var sign = "";
         if (victim < 0)
         {
            sign = "-";
            victim = -victim;
            amount--;
         }
         return sign + victim.ToString().PadLeft(amount, '0');
      }

      public Value Chr() => ((char)number).ToString();

      public Value Range() => new NSIntRange(0, (int)number, false);

      public Value Over()
      {
         var start = (int)number;
         var count = Arguments[0].Int;
         var stop = start + count - 1;

         var array = new Array();
         for (var i = start; i <= stop; i++)
            array.Add(i);

         return array;
      }

      public Value Over(int count, int increment)
      {
         var start = (int)number;
         var array = new Array();
         if (count == 0)
            return new Array();

         var stop = start + count - 1;
         if (increment > 0)
            for (var i = start; i <= stop; i += increment)
               array.Add(i);
         else
            for (var i = start; i >= stop; i += increment)
               array.Add(i);

         return array;
      }

      public Value To()
      {
         var stop = (int)Arguments[0].Number;
         var incrementValue = Arguments[1];
         var increment = incrementValue.IsEmpty ? 1 : (int)incrementValue.Number;
         return To(stop, increment);
      }

      public Value To(int stop, int increment)
      {
         var start = (int)number;
         if (start > stop)
            increment = -Abs(increment);
         var array = new Array();
         if (stop == 0 && increment > 0)
            return array;

         if (increment > 0)
            for (var i = start; i <= stop; i += increment)
               array.Add(i);
         else
            for (var i = start; i >= stop; i += increment)
               array.Add(i);

         return array;
      }

      public Value Times()
      {
         var block = Arguments.Executable;
         if (block.CanExecute)
         {
            var count = (int)Math.Min(number, MAX_LOOP);
            for (var i = 0; i < count; i++)
               block.Evaluate();
         }

         return null;
      }

      public Value Random()
      {
         switch ((int)number)
         {
            case 0:
               return State.Random();
            case 1:
               return State.RandomInt();
            default:
               return State.Random((int)number);
         }
      }

      public Value Round() => Math.Round(number, (int)Arguments[0].Number);

      public Value Max() => Math.Max(number, Arguments[0].Number);

      public Value Min() => Math.Min(number, Arguments[0].Number);

      public Value Atan2() => Math.Atan2(number, Arguments[0].Number);

      public override Value Clone() => new Double(number);

      public Value CloseTo()
      {
         var other = Arguments[0].Number;
         var delta = Arguments[1].Number;
         return Abs(number - other) <= delta;
      }

      public Value Concat() => Text + Arguments[0].Text;

      public Value Factorial()
      {
         var n = (int)number;
         if (n <= 2)
            return n;

         double result = 1;
         for (var i = 2; i <= n; i++)
            result *= i;

         return result;
      }

      public Value IsPrime()
      {
         var n = (int)number;
         if ((n & 1) == 0)
            return n == 2;

         for (var i = 3; i * i <= n; i += 2)
            if (n % i == 0)
               return false;

         return n != 1;
      }

      public Value Words()
      {
         if ((int)number == 0)
            return "zero";

         if (nums == null)
            nums = STR_WORDS.Split("/s+");
         if (tens == null)
            tens = "ten twenty thirty forty fifty sixty seventy eighty ninety".Split("/s+");
         return intoWords((int)number);
      }

      string intoWords(int n)
      {
         if (n >= 1000)
            return intoWords(n / 1000) + " thousand " + intoWords(n % 1000);
         if (n >= 100)
            return intoWords(n / 100) + " hundred " + intoWords(n % 100);
         if (n >= 20)
            return tens[n / 10 - 1] + " " + intoWords(n % 10);

         return nums[n - 1];
      }

      public Value Fraction() => number - (int)number;

      public Value Seq() => new ScalarStream(1, (int)number);

      public Value FloorDiv()
      {
         var divisor = Arguments[0].Number;
         Reject((int)divisor == 0, LOCATION, "Divide by 0");
         return Floor(number / divisor);
      }

      public Value GCD()
      {
         var a = (int)number;
         var b = Arguments[0].Int;
         return gcd(a, b);
      }

      static int gcd(int a, int b)
      {
         while (b != 0)
         {
            var rem = a % b;
            a = b;
            b = rem;
         }

         return a;
      }

      public Value LCM()
      {
         var a = (int)number;
         var b = Arguments[0].Int;
         return a * b / gcd(a, b);
      }

      public Value Add() => number + Arguments[0].Number;

      public Value Sub() => number - Arguments[0].Number;

      public Value Mult() => number * Arguments[0].Number;

      public Value Div() => number / Arguments[0].Number;

      public Value Mod() => number % Arguments[0].Number;

      public Value Pow() => Math.Pow(number, Arguments[0].Number);

      public Value Rat()
      {
         var error = Arguments[0].Number;
         if (error == 0)
            error = 0.000001;
         var n = (int)Floor(number);
         var x = number - n;
         if (x < error)
            return new Rational(n, 1);
         if (1 - error < x)
            return new Rational(n + 1, 1);

         var lowerN = 0;
         var lowerD = 1;
         var upperN = 1;
         var upperD = 1;

         var numberPlusError = x + error;
         var numberMinusError = x - error;

         while (true)
         {
            var middleN = lowerN + upperN;
            var middleD = lowerD + upperD;
            if (middleD * numberPlusError < middleN)
            {
               upperN = middleN;
               upperD = middleD;
            }
            else if (middleN < numberMinusError * middleD)
            {
               lowerN = middleN;
               lowerD = middleD;
            }
            else
               return new Rational(n * middleD + middleN, middleD);
         }
      }

      public Value Values() => createArrayFromNumber();

      Array createArrayFromNumber()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return (Array)Range().SourceArray;

            assistant.IteratorParameter();
            var array = new Array();
            foreach (var i in Enumerable.Range(0, (int)number))
            {
               assistant.SetIteratorParameter(i);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return array;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               array.Add(value);
            }

            return array;
         }
      }

      public override int GetHashCode() => number.GetHashCode();

      public Value List() => ArrayToList(createArrayFromNumber());

      public Value Lazy() => new NSLazyRange(this, AddOne());

      public Array ToArray() => GeneratorToArray(this);

      public override IMaybe<INSGenerator> PossibleIndexGenerator() => none<INSGenerator>();

      public Value Big() => new Big(number);
   }
}