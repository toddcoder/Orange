using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class AllFramework : GeneratorFramework
	{
		bool all;

		public AllFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments) => all = true;

	   public override Value Map(Value value)
		{
			if (!block.Evaluate().IsTrue)
				all = false;
			return all;
		}

		public override bool Exit(Value value) => !all;

	   public override Value ReturnValue() => all;
	}
}