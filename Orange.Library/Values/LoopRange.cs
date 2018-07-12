using System;
using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Standard.Types.Lambdas.LambdaFunctions;

namespace Orange.Library.Values
{
   public class LoopRange : Value, INSGeneratorSource, ISharedRegion
   {
      string variable;
      Block init;
      bool positive;
      Block condition;
      Block increment;
      IMaybe<Block> yielding;
      Region region;
      Func<bool> conditionFunc;

      public LoopRange(string variable, Block init, bool positive, Block condition, Block increment, IMaybe<Block> yielding,
         Region region = null)
      {
         this.variable = variable;
         this.init = init;
         this.positive = positive;
         this.condition = condition;
         this.increment = increment;
         this.yielding = yielding;
         this.region = region ?? new Region();
         conditionFunc = this.positive ? func(() => condition.IsTrue) : func(() => !condition.IsTrue);
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Range;

      public override bool IsTrue => true;

      public override Value Clone()
      {
         return new LoopRange(variable, (Block)init.Clone(), positive, (Block)condition.Clone(), (Block)increment.Clone(),
            yielding.Map(y => (Block)y.Clone()), region.Clone());
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "array", v => ((LoopRange)v).ToArray());
      }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index)
      {
         using (var popper = new SharedRegionPopper(region, this, "loop-range-next"))
         {
            popper.Push();
            if (index == 0)
               region.SetParameter(variable, init.Evaluate());
            if (conditionFunc())
            {
               var result = yielding.FlatMap(y => y.Evaluate(), () => region[variable]);
               region[variable] = increment.Evaluate();
               return result;
            }

            return NilValue;
         }
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public Region Region
      {
         get => region;
         set => region = value.Clone();
      }

      public Region SharedRegion { get; set; }

      public override string ToString()
      {
         var direction = positive ? "while" : "until";
         var yieldingAs = yielding.FlatMap(y => y.ToString(), () => variable);
         return $"(loop {variable} = {init} {direction} then {increment} yield {yieldingAs})";
      }

      public override Value AssignmentValue() => ToArray();

      public override Value AlternateValue(string message) => AssignmentValue();

      public override Value ArgumentValue() => AssignmentValue();
   }
}