using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class UniqueFramework : GeneratorFramework
	{
		Array array;

		public UniqueFramework(Generator generator, Arguments arguments)
			: base(generator, null, arguments) => array = new Array();

	   public override Value Map(Value value)
		{
			if (array.ContainsValue(value))
				return null;
			array.Add(value);
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => array;
	}
}