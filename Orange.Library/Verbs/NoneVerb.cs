using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class NoneVerb : GeneratorTermination
   {
      public override Value Evaluate(NSIterator iterator, Lambda lambda)
      {
         iterator.Reset();
         var value = iterator.Next();

         for (var i = 0; !value.IsNil && i < MAX_LOOP; i++)
         {
            var result = lambda.Invoke(new Arguments(value));
            if (result.IsTrue)
               return false;
            value = iterator.Next();
         }
         return true;
      }

      public override string Location => "none";
   }
}