using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class KeepFramework : GeneratorFramework
	{
		Array trueArray;
		Array falseArray;

		public KeepFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments)
		{
			trueArray = new Array();
			falseArray = new Array();
		}

		public override Value Map(Value value)
		{
			if (block.Evaluate().IsTrue)
				trueArray.Add(value);
			else
				falseArray.Add(value);
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => new Array
		{
		   trueArray,
		   falseArray
		};
	}
}