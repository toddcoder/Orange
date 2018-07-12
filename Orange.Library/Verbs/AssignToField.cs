using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class AssignToField : Verb, IStatement
   {
      string fieldName;
      Block expression;
      bool reference;
      string result;
      string typeName;

      public AssignToField(string fieldName, Block expression, bool reference)
      {
         this.fieldName = fieldName;
         this.expression = expression;
         this.reference = reference;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         Assert(Regions.FieldExists(fieldName), "Assign to field", $"{fieldName} doesn't exist");
         var assignmentValue = expression.Evaluate().AssignmentValue();
         if (reference)
            fieldName = Regions[fieldName].Text;
         Regions[fieldName] = assignmentValue;
         result = assignmentValue.ToString();
         typeName = assignmentValue.Type.ToString();
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"{fieldName} = {expression}";

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public string FieldName => fieldName;

      public Block Expression => expression;
   }
}