using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class MinFramework : GeneratorFramework
	{
		Value min;
		bool evaluate;

		public MinFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
			min = null;
			evaluate = block.CanExecute;
		}

		public override Value Map(Value value)
		{
			if (evaluate)
			{
				if (min == null)
					min = value;
				else
					RegionManager.Regions.SetParameter(parameterName, min);
				var minResult = block.Evaluate();
				RegionManager.Regions.SetParameter(parameterName, value);
				var valueResult = block.Evaluate();
				if (valueResult.Compare(minResult) < 0)
					return min = value;
				return value;
			}
			if (min == null || value.Compare(min) < 0)
				min = value;
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => min;
	}
}