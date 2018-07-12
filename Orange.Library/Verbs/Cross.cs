using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Cross : Verb
   {
      const string LOCATION = "Cross";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var right = stack.Pop(true, LOCATION);
         var left = stack.Pop(true, LOCATION);

         var leftGenerator = left.PossibleGenerator();
         var rightGenerator = right.PossibleGenerator();
         Assert(leftGenerator.IsSome, LOCATION, $"{left} is not a generator");
         Assert(rightGenerator.IsSome, LOCATION, $"{right} is not a generator");

         var leftIterator = new NSIterator(leftGenerator.Value);
         var rightIterator = new NSIterator(rightGenerator.Value);
         var outerArray = new Array();

         var outerValue = leftIterator.Next();
         for (var i = 0; !outerValue.IsNil && i < MAX_ARRAY; i++)
         {
            rightIterator.Reset();
            var innerValue = rightIterator.Next();
            for (var j = 0; !innerValue.IsNil && j < MAX_ARRAY; j++)
            {
               var innerArray = new Array {outerValue, innerValue};
               outerArray.Add(modifyInnerArray(innerArray));
               innerValue = rightIterator.Next();
            }
            outerValue = leftIterator.Next();
         }
         return outerArray;
      }

      protected virtual Value modifyInnerArray(Array array) => array;

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;
   }
}