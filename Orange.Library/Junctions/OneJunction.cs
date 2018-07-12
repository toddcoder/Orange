using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Junctions
{
   public class OneJunction : Junction
   {
      protected bool found;

      public OneJunction(INSGenerator generator, Arguments arguments)
         : base(generator, arguments) => found = false;

      public override bool IfNil() => found;

      public override IMaybe<bool> Success()
      {
         if (found)
            return false.Some();
         found = true;
         return none<bool>();
      }

      public override bool Final() => found;
   }
}