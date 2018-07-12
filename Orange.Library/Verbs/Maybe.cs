using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class Maybe : Verb, IStatement
   {
      string fieldName;
      Block expression;
      Block ifTrue;
      IMaybe<Block> ifFalse;
      IMaybe<Block> guardBlock;
      string result;
      string typeName;

      public Maybe(string fieldName, Block expression, Block ifTrue, IMaybe<Block> ifFalse, IMaybe<Block> guardBlock)
      {
         this.fieldName = fieldName;
         this.expression = expression;
         this.ifTrue = ifTrue;
         this.ifFalse = ifFalse;
         this.guardBlock = guardBlock;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var evaluated = expression.Evaluate();
         Value returned;
         if (evaluated is None)
         {
            if (guardBlock.IsSome)
            {
               returned = ifTrue.Evaluate();
               result = ifTrue.ToString();
               typeName = returned.Type.ToString();
               return returned;
            }

            if (ifFalse.IsSome)
            {
               returned = ifFalse.Value.Evaluate();
               result = ifFalse.ToString();
               typeName = returned.Type.ToString();
               return returned;
            }

            return null;
         }

         //var maybe = evaluated.As<Some>();
         if (evaluated is Some)
         {
            if (ifFalse.IsSome)
            {
               returned = ifFalse.Value.Evaluate();
               result = returned.ToString();
               typeName = returned.Type.ToString();
               return returned;
            }

            return null;
         }

         //var value = maybe.Value();
         if (guardBlock.IsSome)
         {
            //Regions.Current.SetParameter(fieldName, value);
            if (guardBlock.Value.IsTrue)
            {
               result = guardBlock.Value.ToString();
               typeName = guardBlock.Value.Type.ToString();
               return null;
            }

            returned = ifTrue.Evaluate();
            result = ifTrue.ToString();
            typeName = returned.Type.ToString();
            return returned;
         }

         using (var popper = new RegionPopper(new Region(), "maybe"))
         {
            popper.Push();
            //Regions.Current.SetParameter(fieldName, value);
            returned = ifTrue.Evaluate();
            result = ifTrue.ToString();
            typeName = returned.Type.ToString();
         }
         return returned;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"maybe {fieldName} = {expression} ({ifTrue}){ifFalse.FlatMap(f => $" ({f})", () => "")}";

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }
   }
}