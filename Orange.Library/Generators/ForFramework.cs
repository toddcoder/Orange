using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class ForFramework : GeneratorFramework
	{
		public ForFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
		}

		public override Value Map(Value value)
		{
			block.Evaluate();
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => generator;
	}
}