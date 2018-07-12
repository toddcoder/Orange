using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public abstract class OneValueVerb : Verb
   {
      public override Value Evaluate()
      {
         var value = Runtime.State.Stack.Pop(true, Location);
         return Evaluate(value);
      }

      public abstract Value Evaluate(Value value);

      public abstract string Location
      {
         get;
      }
   }
}