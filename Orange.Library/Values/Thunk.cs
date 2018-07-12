using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Values
{
   public class Thunk : Value
   {
      Block block;
      Region region;

      public Thunk(Block block, Region region = null)
      {
         this.block = block;
         this.block.Expression = true;
         this.region = region;
      }

      Value getValue() => block.Evaluate(region ?? Regions.Current);

      public override int Compare(Value value) => getValue().Compare(value);

      public override string Text
      {
         get { return getValue().Text; }
         set { }
      }

      public override double Number
      {
         get { return getValue().Number; }
         set { }
      }

      public override ValueType Type => ValueType.Thunk;

      public override bool IsTrue => getValue().IsTrue;

      public override Value Clone() => new Thunk((Block)block.Clone());

      protected override void registerMessages(MessageManager manager) { }

      public override Value AlternateValue(string message) => getValue();

      public override string ToString() => $"({block})";

      public Block Block => block;

      public Region Region
      {
         get => region;
         set => region = value;
      }

      public override IMaybe<INSGenerator> PossibleGenerator() => getValue().PossibleGenerator();

      public override IMaybe<INSGenerator> PossibleIndexGenerator() => getValue().PossibleIndexGenerator();

      public override bool ProvidesGenerator => getValue().ProvidesGenerator;
   }
}