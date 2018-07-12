using static Orange.Library.Compiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class ObjectIndexer : Variable
	{
		Object obj;
		Block indexes;

		public ObjectIndexer(Object obj, Block indexes)
			: base(VAR_ANONYMOUS + CompilerState.ObjectID())
		{
			this.obj = obj;
			this.indexes = indexes;
		}

		Arguments getArguments()
		{
			var builder = new CodeBuilder();
			builder.Argument(indexes);
			return builder.Arguments;
		}

		Arguments getArguments(Value value)
		{
			var builder = new CodeBuilder();
			builder.Argument(indexes);
			builder.ValueAsArgument(value);
			return builder.Arguments;
		}

		public override Value Value
		{
			get
			{
				var name = LongToMangledPrefix("get", "item");
				return SendMessage(obj, name, getArguments());
			}
			set
			{
				if (obj.ID == value.ID)
					return;

				var name = LongToMangledPrefix("set", "item");
				SendMessage(obj, name, getArguments(value));
			}
		}

		public override Value AlternateValue(string message) => Value;

	   public override string ContainerType => ValueType.ObjectIndexer.ToString();

	   public override bool IsIndexer => true;
	}
}