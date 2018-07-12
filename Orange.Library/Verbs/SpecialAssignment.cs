using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Values.Value;
using Signature = Orange.Library.Values.Signature;

namespace Orange.Library.Verbs
{
   public class SpecialAssignment : Verb
   {
      string variableName;
      Value value;
      string sourceVariableName;

      public SpecialAssignment(string variableName, Value value)
      {
         this.variableName = variableName;
         this.value = value;
         sourceVariableName = null;
      }

      public SpecialAssignment(string variableName, string sourceVariableName)
      {
         this.variableName = variableName;
         value = null;
         this.sourceVariableName = sourceVariableName;
      }

      public SpecialAssignment()
         : this("", "") { }

      public override Value Evaluate()
      {
         if (sourceVariableName != null)
            value = Regions[sourceVariableName];
         Regions.CreateVariable(variableName);
         var variable = new Variable(variableName);
         value.AssignTo(variable);
         return null;
      }

      public IMaybe<Signature> Signature => value.IfCast<Abstract>().Map(a => a.Signature);

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => value.ToString();

      public ValueType Type => value?.Type ?? ValueType.Class;

      public string VariableName => variableName;

      public bool IsAbstract => value.Type == ValueType.Abstract;

      public Value Value => value;
   }
}