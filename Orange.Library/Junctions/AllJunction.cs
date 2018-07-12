using Orange.Library.Values;
using Standard.Types.Maybe;

namespace Orange.Library.Junctions
{
   public class AllJunction : Junction
   {
      public AllJunction(INSGenerator generator, Arguments arguments)
         : base(generator, arguments)
      {
      }

      public override bool IfNil() => true;

      public override bool Compare(Value current, Value value) => !base.Compare(current, value);

      public override bool BlockCompare(Block block) => !base.BlockCompare(block);

      public override IMaybe<bool> Success() => false.Some();

      public override bool Final() => true;
   }
}