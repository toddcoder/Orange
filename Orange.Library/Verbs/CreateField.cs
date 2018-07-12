using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;

namespace Orange.Library.Verbs
{
   public class CreateField : Verb, IStatement
   {
      bool readOnly;
      string fieldName;
      VisibilityType visibility;
      string type;

      public CreateField(bool readOnly, string fieldName, VisibilityType visibility)
      {
         this.readOnly = readOnly;
         this.fieldName = fieldName;
         this.visibility = visibility;
         type = this.readOnly ? "val" : "var";
      }

      public override Value Evaluate()
      {
         var fn = fieldName;
         var only = readOnly;
         var visibilityType = visibility;
         CreateFieldInCurrentRegion(fn, only, visibilityType);
         return null;
      }

      public static void CreateFieldInCurrentRegion(string fieldName, bool readOnly, VisibilityType visibility)
      {
         var current = Regions.Current;
         Reject(current.Exists(fieldName), "Create field", $"{fieldName} already exists");
         if (readOnly)
            current.CreateReadOnlyVariable(fieldName, visibility: visibility);
         else
            current.CreateVariable(fieldName, visibility: visibility);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"{type} {fieldName}";

      public string Result => fieldName;

      public string TypeName => "";

      public int Index { get; set; }
   }
}