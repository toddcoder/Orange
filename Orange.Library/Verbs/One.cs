using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class One : GeneratorTermination
   {
      public override Value Evaluate(NSIterator iterator, Lambda lambda)
      {
         var found = false;

         iterator.Reset();
         var value = iterator.Next();

         for (var i = 0; !value.IsNil && i < MAX_LOOP; i++)
         {
            var result = lambda.Invoke(new Arguments(value));
            if (result.IsTrue)
            {
               if (found)
                  return false;
               found = true;
            }
            value = iterator.Next();
         }
         return found;
      }

      public override string Location => "one";
   }
}