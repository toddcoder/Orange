using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class ClassSetter : Setter
   {
      const string LOCATION = "Class setter";

      public ClassSetter(string message, IMatched<Verb> verb, Block expression)
         : base(message, verb, expression)
      {
      }

      public override Value Evaluate()
      {
         var cls = Regions["class"];
         Reject(cls.IsEmpty, LOCATION, $"{message} message called out of class");
         State.Stack.Push(cls);
         return base.Evaluate();
      }
   }
}