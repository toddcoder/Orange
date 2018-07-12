using Orange.Library.Managers;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class NSLazyRange : Value, INSGeneratorSource, IRangeEndpoints
   {
      public class NSLazyRangeGenerator : NSGenerator
      {
         protected Value seed;
         protected Lambda increment;
         protected Value current;

         public NSLazyRangeGenerator(INSGeneratorSource generatorSource, Value seed, Lambda increment)
            : base(generatorSource)
         {
            this.seed = seed;
            this.increment = increment;
         }

         public override void Reset()
         {
            base.Reset();
            current = NilValue;
         }

         public override Value Next()
         {
            if (++index >= MAX_LOOP)
               return NilValue;

            return current = index == 0 ? seed : increment.Invoke(new Arguments(current));
         }
      }

      protected Value seed;
      protected Lambda increment;

      public NSLazyRange(Value seed, Lambda increment)
      {
         this.seed = seed;
         this.increment = increment;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return GeneratorToArray(this).Text; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => true;

      public override Value Clone() => new NSLazyRange(seed, increment);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "array", v => ((NSLazyRange)v).ToArray());
      }

      public INSGenerator GetGenerator() => new NSLazyRangeGenerator(this, seed, increment);

      public Value Next(int index) => index < MAX_LOOP ? increment.Evaluate(new Arguments(index)) : NilValue;

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public override string ToString() => $"{seed} .* {increment}";

      public override Value AlternateValue(string message) => (Value)GetGenerator();

      public override bool IsArray => true;

      public override Value SourceArray => ToArray();

      public int Start(int length) => seed.Int;

      public int Stop(int length) => length + seed.Int;

      public int Increment(int length) => increment.Invoke(new Arguments(seed)).Int - seed.Int;

      public bool Inclusive => true;
   }
}