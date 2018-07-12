using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;
using static Orange.Library.Verbs.CreateClass;

namespace Orange.Library.Verbs
{
   public class CreateStaticBlock : Verb
   {
      const string LOCATION = "Create static block";

      string className;
      Block staticBlock;

      public CreateStaticBlock(string className, Block staticBlock)
      {
         this.className = className;
         this.staticBlock = staticBlock;
      }

      public override Value Evaluate()
      {
         Assert(className.IsNotEmpty(), LOCATION, "No class name provided");
         if (!Regions.VariableExists(className))
         {
            var newClass = new Class(new Parameters(), new Block(), null, "", new string[0], new Parameters(), false);
            Create(className, newClass);
         }
         Assert(Regions.VariableExists(className), LOCATION, $"Class {className} doesn't exist");
         var value = Regions[className];
         Assert(value.Type == ValueType.Class, LOCATION, $"{className} isn't a class");
         var cls = (Class)value;
         cls.ClassBlock = staticBlock;
         cls.StaticObject = null;
         cls.CreateStaticObject();
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"static {className}{{{staticBlock}}}";
   }
}