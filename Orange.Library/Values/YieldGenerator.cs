using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Verbs;

namespace Orange.Library.Values
{
   public class YieldGenerator : Value, IGenerator
   {
      static Lambda[] convertToLambdas(Block block)
      {
         var lambdas = new List<Lambda>();
         var builder = new CodeBuilder();
         var yielding = false;

         Block innerBlock;
         foreach (var verb in block.AsAdded)
         {
            builder.Verb(verb);
            if (yielding)
            {
               if (verb is End)
               {
                  innerBlock = builder.Block;
                  lambdas.Add(Lambda.FromBlock(innerBlock));
                  builder = new CodeBuilder();
                  yielding = false;
               }
            }
            else if (verb is Yield)
               yielding = true;
         }

         innerBlock = builder.Block;
         if (innerBlock.Count > 0)
            lambdas.Add(Lambda.FromBlock(innerBlock));

         return lambdas.ToArray();
      }

      Lambda[] lambdas;

      public YieldGenerator(Block block) => lambdas = convertToLambdas(block);

      public YieldGenerator(Lambda[] lambdas) => this.lambdas = lambdas;

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.YieldGenerator;

      public override bool IsTrue => false;

      public override Value Clone() => new YieldGenerator(lambdas);

      protected override void registerMessages(MessageManager manager) { }

      public void Before() { }

      public Value Next(int index)
      {
         if (index >= lambdas.Length)
            return new Nil();

         return lambdas[index].Evaluate();
      }

      public void End() { }
   }
}