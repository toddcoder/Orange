using Orange.Library.Values;

namespace Orange.Library.Generators
{
	public class ArrayFramework : GeneratorFramework
	{
		Array array;

		public ArrayFramework(Generator generator, Arguments arguments)
			: base(generator, null, arguments) => array = new Array();

	   public override Value Map(Value value)
		{
			if (!value.IsNil)
				array.Add(value);
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue() => array;
	}
}