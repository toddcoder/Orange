using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library
{
   public class ClassBuilder
   {
      public static Value Invoke(string className, Arguments arguments)
      {
         var possibleClass = Regions[className];
         var _class = possibleClass as Class;
         RejectNull(_class, "Class Builder", $"{className} isn't a class");
         return SendMessage(_class, "invoke", arguments);
      }

      public static Value Invoke(string className, Value value) => Invoke(className, new Arguments(value));

      public static Value Invoke(string className) => Invoke(className, new Arguments());

      string className;
      string superName;
      CodeBuilder builder;
      Parameters parameters;
      Parameters superParameters;
      Block objectBlock;
      Block classBlock;
      Block helperBlock;
      List<string> traitNames;
      List<CreateFunction> helperFunctions;

      public ClassBuilder(string className, string superName = "")
      {
         this.className = className;
         this.superName = superName;
         builder = new CodeBuilder();
         parameters = new Parameters();
         objectBlock = new Block();
         classBlock = new Block();
         helperBlock = null;
         LockedDown = false;
         traitNames = new List<string>();
         helperFunctions = null;
      }

      public bool LockedDown { get; set; }

      public void Parameter(string name, Value defaultValue = null,
         Object.VisibilityType visibility = Object.VisibilityType.Public, bool readOnly = false, bool lazy = false)
      {
         builder.Parameter(name, defaultValue, visibility, readOnly, lazy);
      }

      public void Parameter(Parameter parameter) => builder.Parameter(parameter);

      public void Parameters(Parameters parameters) => builder.Parameters(parameters);

      public void Parameters() => parameters = builder.Parameters();

      public void SuperParameters() => superParameters = builder.Parameters();

      public CodeBuilder Builder => builder;

      public void BeginObjectBlock() => builder.Push();

      public void EndObjectBlock() => objectBlock = builder.Pop(false);

      public void BeginClassBlock() => builder.Push();

      public void EndClassBlock() => classBlock = builder.Pop(false);

      public void BeginHelperBlock() => builder.Push();

      public void EndHelperBlock() => helperBlock = builder.Pop(false);

      public void AddTraitName(string traitName) => traitNames.Add(traitName);

      public void AddHelperFunction(string functionName, bool multiCapable = false,
         Object.VisibilityType visibilityType = Object.VisibilityType.Public, bool _override = false, bool global = false,
         bool autoInvoke = false)
      {
         var createFunction = builder.CreateFunction(functionName, multiCapable, visibilityType, _override, global);
         if (helperFunctions == null)
            helperFunctions = new List<CreateFunction>();
         helperFunctions.Add(createFunction);
      }

      public void Create()
      {
         var _class = new Class(parameters, objectBlock, classBlock, superName, traitNames.ToArray(), superParameters,
            LockedDown);
         CreateClass.Create(className, _class, helperFunctions, helperBlock);
      }

      public void Constant(string name, Value value)
      {
         builder.Define(name, readOnly: true);
         builder.Variable(name);
         builder.Assign();
         builder.Value(value);
         builder.End();
      }
   }
}