using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class NoneFramework : GeneratorFramework
	{
		bool none;

		public NoneFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments) => none = true;

	   public override Value Map(Value value)
		{
			if (block.Evaluate().IsTrue)
				none = false;
			return none;
		}

		public override bool Exit(Value value) => !none;

	   public override Value ReturnValue() => none;
	}
}