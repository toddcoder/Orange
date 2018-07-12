using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class With : Verb, IStatement
   {
      Block sourceBlock;
      Block actionsBlock;
      VerbPresidenceType presidence;
      string result;
      string typeName;

      public With(Block sourceBlock, Block actionsBlock, VerbPresidenceType presidence)
      {
         this.sourceBlock = sourceBlock;
         this.actionsBlock = actionsBlock;
         this.presidence = presidence;
         result = "";
         typeName = "";
      }

      public With(VerbPresidenceType presidence)
      {
         this.presidence = presidence;
         sourceBlock = new Block();
         actionsBlock = new Block();
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var value = sourceBlock.Evaluate();
         if (value is Object obj)
         {
            var region = obj.Region;
            using (var popper = new RegionPopper(region, "with"))
            {
               popper.Push();
               result = value.ToString();
               typeName = value.Type.ToString();
               actionsBlock.Evaluate();
            }
            return obj;
         }

         return value;
      }

      public override VerbPresidenceType Presidence => presidence;

      public override string ToString() => $"with {sourceBlock} {actionsBlock}";

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }
   }
}