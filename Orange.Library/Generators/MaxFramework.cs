using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class MaxFramework : GeneratorFramework
	{
		Value max;
		bool evaluate;

		public MaxFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
			max = null;
			evaluate = block.CanExecute;
		}

		public override Value Map(Value value)
		{
			if (evaluate)
			{
				if (max == null)
					max = value;
				else
				RegionManager.Regions.SetParameter(parameterName, max);
				var maxResult = block.Evaluate();
				RegionManager.Regions.SetParameter(parameterName, value);
				var valueResult = block.Evaluate();
				if (valueResult.Compare(maxResult) > 0)
					return max = value;
				return value;
			}
			if (max == null || value.Compare(max) > 0)
				max = value;
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => max;
	}
}