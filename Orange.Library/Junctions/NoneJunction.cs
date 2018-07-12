using Standard.Types.Maybe;

namespace Orange.Library.Junctions
{
   public class NoneJunction : Junction
   {
      public NoneJunction(INSGenerator generator, Arguments arguments)
         : base(generator, arguments)
      {
      }

      public override bool IfNil() => true;

      public override IMaybe<bool> Success() => false.Some();

      public override bool Final() => true;
   }
}