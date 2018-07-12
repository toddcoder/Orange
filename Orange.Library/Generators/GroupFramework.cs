using Orange.Library.Values;
using Standard.Types.Collections;

namespace Orange.Library.Generators
{
	public class GroupFramework : GeneratorFramework
	{
		Hash<string, Array> groups;

		public GroupFramework(Generator generator, Block block, Arguments arguments)
			: base(generator, block, arguments) => groups = new Hash<string, Array>();

	   public override Value Map(Value value)
		{
			var key = block.Evaluate().Text;
		   var array = groups.Find(key, s => new Array(), true);
			array.Add(value);
			return value;
		}

		public override bool Exit(Value value) => value.IsNil;

	   public override Value ReturnValue()
		{
			var array = new Array();
			foreach (var item in groups)
				array[item.Key] = item.Value;
			return array;
		}
	}
}