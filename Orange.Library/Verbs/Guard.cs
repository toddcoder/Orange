using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class Guard : Verb, IStatement
   {
      Block condition;
      Block block;
      string result;

      public Guard(Block condition, Block block)
      {
         this.condition = condition;
         this.block = block;
         result = "";
      }

      public override Value Evaluate()
      {
         if (!condition.IsTrue)
         {
            result = "guard failed";
            block.Evaluate();
            return null;
         }

         result = "guard success";
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}