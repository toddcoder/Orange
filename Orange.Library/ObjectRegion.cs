using Orange.Library.Values;
using Standard.Types.Collections;
using static Orange.Library.Runtime;
using Object = Orange.Library.Values.Object;

namespace Orange.Library
{
   public class ObjectRegion : Region
   {
      const string LOCATION = "Object region";

      protected Object obj;
      protected Hash<string, IInvokeable> invariants;

      public ObjectRegion(Object obj)
      {
         this.obj = obj;
         invariants = new Hash<string, IInvokeable>();
      }

      public ObjectRegion(Object obj, Hash<string, IInvokeable> invariants)
      {
         this.obj = obj;
         this.invariants = invariants;
      }

      public Object Object => obj;

      public override void SetVariable(string name, Value value)
      {
         var invariant = invariants[name];
         if (invariant == null)
         {
            base.SetVariable(name, value);
            return;
         }

         Value oldValue = new Nil();
         if (variables.ContainsKey(name))
            oldValue = variables[name];
         base.SetVariable(name, value);
         var arguments = new Arguments();
         arguments.AddArgument(oldValue);
         arguments.AddArgument(value);
         var result = obj.Invoke(invariant, arguments);
         Assert(result.IsTrue, LOCATION, $"Invariant failed for {name}");
      }

      public override Region ReferenceClone() => new ObjectRegion(obj, invariants);
   }
}