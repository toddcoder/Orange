using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Strings;

namespace Orange.Library.Generators
{
	public class ScalarFramework : GeneratorFramework
	{
		Value accumulator;
		string parameterName2;

		public ScalarFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
			accumulator = null;
			parameterName2 = parameterName.Succ();
		}

		public override Value Map(Value value)
		{
			if (accumulator == null)
			{
				accumulator = value;
				return value;
			}
			RegionManager.Regions.SetParameter(parameterName, accumulator);
			RegionManager.Regions.SetParameter(parameterName2, value);
			accumulator = block.Evaluate();
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => accumulator;
	}
}