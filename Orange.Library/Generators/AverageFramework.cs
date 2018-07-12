using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class AverageFramework : SumFramework
	{
		protected int n;

		public AverageFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments) => n = 0;

	   public override Value Map(Value value)
		{
			n++;
			return base.Map(value);
		}

		public override Value ReturnValue() => base.ReturnValue().Number / n;
	}
}