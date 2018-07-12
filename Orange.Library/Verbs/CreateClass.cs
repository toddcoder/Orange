using System.Collections.Generic;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class CreateClass : Verb, IStatement
   {
      public static void Create(string className, Class cls, IEnumerable<CreateFunction> helperFunctions = null,
         Block helperBlock = null)
      {
         cls.Name = className;
         var current = Regions.Current;
         current.CreateVariable(className);
         current[className] = cls;
         cls.CreateStaticObject();
         if (helperFunctions != null)
            foreach (var help in helperFunctions)
               help.Evaluate();

         helperBlock?.Evaluate();
      }

      string className;
      Class cls;
      string result;

      public CreateClass(string className, Class cls)
      {
         this.className = className;
         this.cls = cls;
         result = "";
      }

      public IEnumerable<CreateFunction> HelperFunctions { get; set; }

      public Block HelperBlock { get; set; }

      public override Value Evaluate()
      {
         Create(className, cls, HelperFunctions, HelperBlock);
         result = cls.ToString();
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => "Class";

      public int Index { get; set; }
   }
}