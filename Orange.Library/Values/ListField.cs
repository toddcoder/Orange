namespace Orange.Library.Values
{
	public class ListField : Variable
	{
		InternalList list;

		public ListField(string name, InternalList list)
			: base(name) => this.list = list;

	   public override Value Value
		{
			get => list[Name];
		   set => list[Name] = value;
		}

		public override string ContainerType => ValueType.ListField.ToString();

	   public override Value Resolve() => Value;

	   public override string ToString() => Value.ToString();
	}
}