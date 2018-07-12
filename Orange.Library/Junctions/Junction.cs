using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;

namespace Orange.Library.Junctions
{
   public abstract class Junction
   {
      protected INSGenerator generator;
      protected Arguments arguments;

      public Junction(INSGenerator generator, Arguments arguments)
      {
         this.generator = generator;
         this.arguments = arguments;
      }

      public abstract bool IfNil();

      public virtual bool Compare(Value current, Value value) => current.Compare(value) == 0;

      public virtual bool BlockCompare(Block block) => block.IsTrue;

      public abstract IMaybe<bool> Success();

      public abstract bool Final();

      public virtual Value Evaluate()
      {
         var value = arguments[0];
         var iterator = new NSIterator(generator);
         iterator.Reset();

         var block = arguments.Executable;
         if (block != null && block.Count > 0)
            return EvaluateAsLambda(iterator, arguments.Parameters, block);

         for (var i = 0; i < MAX_LOOP; i++)
         {
            var current = iterator.Next();
            if (current.IsNil)
               return IfNil();
            if (Compare(current, value))
            {
               var success = Success();
               if (success.IsSome)
                  return success.Value;
            }
         }
         return Final();
      }

      public virtual Value EvaluateAsLambda(NSIterator iterator, Parameters parameters, Block block)
      {
         using (var popper = new RegionPopper(new Region(), "any-of"))
         {
            popper.Push();

            for (var i = 0; i < MAX_LOOP; i++)
            {
               var current = iterator.Next();
               if (current.IsNil)
                  return IfNil();
               parameters.SetValues(current, i);
               if (BlockCompare(block))
               {
                  var success = Success();
                  if (success.IsSome)
                     return success.Value;
               }
            }
            return Final();
         }
      }
   }
}