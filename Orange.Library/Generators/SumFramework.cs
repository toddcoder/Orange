using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class SumFramework : GeneratorFramework
	{
		protected double sum;
		protected bool evaluate;

		public SumFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
			sum = 0;
			evaluate = block.CanExecute;
		}

		public override Value Map(Value value)
		{
			if (evaluate)
				sum += block.Evaluate().Number;
			else
				sum += value.Number;
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => sum;
	}
}