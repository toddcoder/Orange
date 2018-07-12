using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Is : Verb
   {
      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, Location);
         var x = stack.Pop(true, Location);
         if (x.ID == y.ID)
            return true;

         if (x is Object obj)
         {
            if (y is Class cls)
               return obj.Class.IsChildOf(cls);

            if (y is Trait trait)
               return obj.Class.ImplementsTrait(trait) || obj.ImplementsInterface(trait);

            Throw(Location, $"{x} isn't an object");
         }

         return false;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "is";

      public virtual string Location => "Is";
   }
}