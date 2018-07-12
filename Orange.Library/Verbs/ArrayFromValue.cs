using System.Linq;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Verbs
{
   public class ArrayFromValue : Verb
   {
      const string LOCATION = "Array from value";

      static Array fromValue(Value source, int count)
      {
         var array = new Array();
         for (var i = 0; i < count; i++)
            array.Add(source);

         return array;
      }

      static Array fromValue(Value source, INSGenerator count)
      {
         var countSource = (Array)count.Array();
         var array = new Array();
         var outerCount = countSource[0].Int;
         var innerCount = countSource[1].Int;
         for (var i = 0; i < outerCount; i++)
            array.Add(fromValue(source, innerCount));

         return array;
      }

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         return x is Lambda lambda ? getArray(lambda, y) : getArray(x, y);
      }

      static Array getArray(Value x, Value y)
      {
         return y.PossibleGenerator().FlatMap(generator => fromValue(x, generator), () => fromValue(x, y.Int));
      }

      static Array getArray(Lambda lambda, Value seed)
      {
         return new Array(Enumerable.Range(0, seed.Int).Select(i => lambda.Invoke(new Arguments(i))).ToArray());
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Repeat;

      public override string ToString() => "xx";
   }
}