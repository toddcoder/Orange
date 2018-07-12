using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Generator;
using static Orange.Library.Values.Generator.IterationControlType;

namespace Orange.Library.Generators
{
	public abstract class GeneratorFramework
	{
		protected Generator generator;
		protected Block block;
		protected string parameterName;
		protected string argumentName;

		public GeneratorFramework(Generator generator, Block block, Arguments arguments)
		{
			this.generator = generator;
			this.block = block;
			parameterName = this.generator.ParameterName;
			if (arguments != null)
				argumentName = arguments.VariableName(0);
		}

		public abstract Value Map(Value value);
		public abstract bool Exit(Value value);
		public abstract Value ReturnValue();

		public Value Evaluate()
		{
			var aGenerator = generator.GetGenerator();
			var looping = true;

			var region = new Region();
			using (var popper = new RegionPopper(region, GetType().Name.CamelToObjectGraphCase()))
			{
				popper.Push();
			   generator.SharedRegion?.CopyAllVariablesTo(region);
			   aGenerator.Before();
				for (var i = 0; i < MAX_ARRAY && looping; i++)
				{
					IterationControlType control;
					var value = generator.GetNext(aGenerator, i, out control);
					if (value.IsNil)
						break;
					switch (control)
					{
						case Continuing:
							SetParameter(parameterName, value, argumentName);
							value = Map(value);
							if (Exit(value))
							{
								aGenerator.End();
								return value;
							}
							break;
						case Skipping:
							continue;
						case Exiting:
							looping = false;
							break;
					}
				}
				aGenerator.End();
				return ReturnValue();
			}
		}
	}
}