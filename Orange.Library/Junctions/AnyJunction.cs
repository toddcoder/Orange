using Standard.Types.Maybe;

namespace Orange.Library.Junctions
{
   public class AnyJunction : Junction
   {
      public AnyJunction(INSGenerator generator, Arguments arguments)
         : base(generator, arguments)
      {
      }

      public override bool IfNil() => false;

      public override IMaybe<bool> Success() => true.Some();

      public override bool Final() => false;
   }
}