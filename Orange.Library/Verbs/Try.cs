using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class Try : Verb, IStatement
   {
      string fieldName;
      Block block;
      string result;
      string typeName;

      public Try(string fieldName, Block block)
      {
         this.fieldName = fieldName;
         this.block = block;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var value = block.TryEvaluate();
         result = value.ToString();
         typeName = value.Type.ToString();
         if (fieldName.IsEmpty())
            return result;

         Regions.Current.CreateAndSet(fieldName, value);
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public override string ToString() => $"try {fieldName.Extend($" {fieldName} = ")} {block}";
   }
}