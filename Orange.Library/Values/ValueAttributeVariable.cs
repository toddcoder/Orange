using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class ValueAttributeVariable : Variable
	{
		Value value;
		string getter;
		string setter;

		public ValueAttributeVariable(string name, Value value, string getter, string setter)
			: base(name)
		{
			this.value = value;
			this.getter = getter;
			this.setter = setter;
		}

		public ValueAttributeVariable(string name, Value value)
			: this(name, value, LongToMangledPrefix("get", name), LongToMangledPrefix("set", name))
		{
		}

		public override Value Value
		{
			get => SendMessage(value, getter);
		   set => SendMessage(this.value, setter, value);
		}

		public override string ContainerType => ValueType.ValueAttributeVariable.ToString();

	   public override Value Resolve() => this;

	   public override string ToString() => Value.ToString();
	}
}