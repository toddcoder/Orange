using Orange.Library.Values;

namespace Orange.Library
{
   public class ParameterBlock
   {
      const string LOCATION = "Parameter block";

      public static ParameterBlock FromExecutable(Value value)
      {
         var closure = value as Lambda;
         if (closure != null)
         {
            closure.Block.Options = closure.Options;
            return new ParameterBlock(closure);
         }

         var block = value as Block;
         return block != null ? new ParameterBlock(block) : null;
      }

      Parameters parameters;
      Block block;

      public ParameterBlock(Parameters parameters, Block block, bool splatting)
      {
         this.parameters = parameters;
         this.block = block;
         Splatting = splatting;
      }

      public ParameterBlock(Block block)
         : this(new NullParameters(), block, false) { }

      public ParameterBlock(Lambda lambda)
         : this(lambda.Parameters, lambda.Block, lambda.Splatting) { }

      public ParameterBlock(Value value)
      {
         if (value is Lambda lambda)
         {
            parameters = lambda.Parameters;
            block = lambda.Block;
         }
         else
         {
            block = (Block)value;
            parameters = new NullParameters();
         }
      }

      public ParameterBlock()
      {
         parameters = new Parameters();
         block = new Block();
         Splatting = false;
      }

      public Parameters Parameters => parameters;

      public Block Block => block;

      public ParameterBlock Clone() => new ParameterBlock((Parameters)parameters.Clone(), (Block)block.Clone(), Splatting);

      public override string ToString() => $"|{Parameters}| {{{Block}}}";

      public bool Splatting { get; set; }
   }
}