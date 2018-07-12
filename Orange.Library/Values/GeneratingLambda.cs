namespace Orange.Library.Values
{
   public class GeneratingLambda : Lambda
   {
      public GeneratingLambda(Region region, Block block, Parameters parameters, bool enclosing)
         : base(region, block, parameters, enclosing)
      {
         this.enclosing = true;
      }

      protected override Value evaluateBlock() => new Block.BlockGenerator(block)
      {
         Region = region
      };
   }
}