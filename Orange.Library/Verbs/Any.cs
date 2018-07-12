using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class AnyVerb : GeneratorTermination
   {
      public override Value Evaluate(NSIterator iterator, Lambda lambda)
      {
         iterator.Reset();
         var value = iterator.Next();

         for (var i = 0; !value.IsNil && i < MAX_LOOP; i++)
         {
            var result = lambda.Invoke(new Arguments(value));
            if (result.IsTrue)
               return true;
            value = iterator.Next();
         }
         return false;
      }

      public override string Location => "any";
   }
}