using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class CreateLambda : Verb
   {
      Parameters parameters;
      Block block;
      bool splatting;

      public CreateLambda(Parameters parameters, Block block, bool splatting)
      {
         this.parameters = parameters;
         this.block = block;
         this.block.Expression = false;
         this.splatting = splatting;
      }

      public override Value Evaluate()
      {
         var lambdaRegion = State.LambdaRegion;
         var region = new Region();
         lambdaRegion.CopyAllVariablesTo(region);
         return new Lambda(region, block, parameters, true) { Splatting = splatting };
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public Parameters Parameters => parameters;

      public Block Block
      {
         get => block;
         set => block = value;
      }

      public bool Splatting => splatting;

      public override string ToString() => $"(({parameters}) {(splatting ? "=>" : "->")} {block})";

      public override AffinityType Affinity => AffinityType.Value;
   }
}