using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class CreateAutoProperty : Verb
   {
      public class AutoPropertyCreator
      {
         string propertyName;
         bool readOnly;

         public AutoPropertyCreator(string propertyName, bool readOnly)
         {
            this.propertyName = propertyName;
            this.readOnly = readOnly;
         }

         public Lambda GetLambda { get; set; }

         public Lambda SetLambda { get; set; }

         public string BackingName { get; set; }

         public string GetterName => Runtime.LongToMangledPrefix("get", propertyName);

         public string SetterName => Runtime.LongToMangledPrefix("set", propertyName);

         public void Create()
         {
            BackingName = Runtime.VAR_MANGLE + propertyName;
            var builder = new CodeBuilder();
            builder.Variable(BackingName);
            GetLambda = builder.Lambda();
            if (!readOnly)
            {
               builder.Clear();
               builder.Parameter("value");
               builder.Variable(BackingName);
               builder.Operator("=");
               builder.Variable("value");
               SetLambda = builder.Lambda();
            }
            else
               SetLambda = null;
         }
      }

      string propertyName;
      bool readOnly;

      public CreateAutoProperty(string propertyName, bool readOnly)
      {
         this.propertyName = propertyName;
         this.readOnly = readOnly;
      }

      public override Value Evaluate()
      {
         var creator = new AutoPropertyCreator(propertyName, readOnly);
         creator.Create();
         var ns = RegionManager.Regions.Current;
         ns.CreateVariable(creator.BackingName, visibility: Object.VisibilityType.Protected, _override: true);
         ns.CreateVariable(creator.GetterName, _override: true);
         ns[creator.GetterName] = creator.GetLambda;
         if (!readOnly)
         {
            ns.CreateVariable(creator.SetterName, _override: true);
            ns[creator.SetterName] = creator.SetLambda;
         }
         return new Variable(creator.BackingName);
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Push;

      public override string ToString() => $"{(readOnly ? "readonly" : "auto")} {propertyName}";
   }
}