using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class Let : Verb, IStatement
   {
      Block comparisand;
      Block condition;
      Block expression;
      string result;
      string typeName;

      public Let(Block comparisand, Block condition, Block expression)
      {
         this.comparisand = comparisand;
         this.condition = condition;
         this.expression = expression;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var left = expression.Evaluate();
         var right = comparisand.Evaluate();
         if (Case.Match(left, right, Regions.Current, false, false, condition, assigning: true))
         {
            result = left.ToString();
            typeName = left.Type.ToString();
         }
         else
         {
            result = "unassigned";
            typeName = "";
         }
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public override string ToString() => $"let {comparisand} = {expression}";
   }
}