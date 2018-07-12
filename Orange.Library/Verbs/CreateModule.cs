using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;
using static Orange.Library.Values.Object.VisibilityType;
using static Standard.Types.Strings.StringFunctions;

namespace Orange.Library.Verbs
{
   public class CreateModule : Verb, IStatement
   {
      string objectName;
      Class builder;
      bool assignObject;
      VisibilityType visibility;
      string result;

      public CreateModule(string objectName, Class builder, bool assignObject, VisibilityType visibility = Public)
      {
         this.objectName = objectName;
         this.builder = builder;
         this.assignObject = assignObject;
         this.visibility = visibility;
         result = "";
      }

      public override Value Evaluate()
      {
         var id = objectName.IsEmpty() ? UniqueID() : objectName;
         var className = $"{VAR_MANGLE}{id.ToTitleCase()}Module";
         var current = Regions.Current;
         current.CreateVariable(className, visibility: visibility);
         current[className] = builder;
         builder.Name = className;
         builder.CreateStaticObject();
         var obj = builder.NewObject(new Arguments());
         if (assignObject)
         {
            current.CreateVariable(objectName);
            current[objectName] = obj;
         }
         result = $"module {objectName}";
         return obj;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => "Module";

      public int Index { get; set; }
   }
}