using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class DefineExpression : Verb, IStatement
   {
      const string LOCATION = "Define expression";

      string fieldName;
      Thunk thunk;
      string result;

      public DefineExpression(string fieldName, Thunk thunk)
      {
         this.fieldName = fieldName;
         this.thunk = thunk;
         result = "";
      }

      public override Value Evaluate()
      {
         Reject(Regions.FieldExists(fieldName), LOCATION, $"Field {fieldName} is already defined");
         Regions.CreateReadOnlyVariable(fieldName);
         Regions[fieldName] = thunk;
         result = thunk.ToString();
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => $"def {fieldName} = {thunk}";

      public string Result => result;

      public string TypeName => "Thunk";

      public int Index { get; set; }
   }
}