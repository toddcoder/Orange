using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class FindFramework : GeneratorFramework
	{
		public FindFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
		}

		public override Value Map(Value value) => block.Evaluate().IsTrue ? new Some(value) : null;

	   public override bool Exit(Value value) => value != null;

	   public override Value ReturnValue() => new None();
	}
}