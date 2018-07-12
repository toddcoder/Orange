namespace Orange.Library.Values
{
	public class ObjectDynamicVariable : Variable
	{
		Value obj;

		public ObjectDynamicVariable(string name, Value obj)
			: base(name) => this.obj = obj;

	   public override Value Value
		{
			get => Runtime.SendMessage(obj, "get", Name);
		   set
			{
				var arguments = new Arguments();
				arguments.AddArgument(Name);
				arguments.AddArgument(value);
				Runtime.SendMessage(obj, "set", arguments);
			}
		}

		public override string ContainerType => ValueType.ObjectDynamicVariable.ToString();

	   public override Value Resolve() => this;

	   public override string ToString() => Value.ToString();
	}
}