using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class SingleValueArray : Verb
   {
      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "Single value array");
         if (value.Type == ValueType.Array)
            return value;
         var possibleGenerator = value.PossibleIndexGenerator();
         if (possibleGenerator.IsSome)
            return possibleGenerator.Value.Array();
         var array = new Array { value };
         return array;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "@";
   }
}