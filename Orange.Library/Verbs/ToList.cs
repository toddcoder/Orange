using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class ToList : Verb
   {
      Block block;

      public ToList(Block block) => this.block = block;

      public override Value Evaluate()
      {
         if (block.Count == 0)
            return new List();

         var value = block.Evaluate();
         if (value is List list)
            return list;
         if (value.IsArray)
            return List.FromArray((Array)value.SourceArray);

         return List.FromValue(value);
      }

      public Block Block => block;

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => $"[{block}]";
   }
}