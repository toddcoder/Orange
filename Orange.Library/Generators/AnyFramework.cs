using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class AnyFramework : GeneratorFramework
	{
		bool any;
		public AnyFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments) => any = false;

	   public override Value Map(Value value)
		{
			return any = block.Evaluate().IsTrue;
		}

		public override bool Exit(Value value) => any;

	   public override Value ReturnValue() => any;
	}
}