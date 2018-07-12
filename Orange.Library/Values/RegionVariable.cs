namespace Orange.Library.Values
{
	public class RegionVariable : Variable
	{
		Region region;

		public RegionVariable(string name, Region region)
			: base(name) => this.region = region;

	   public override Value Value
		{
			get => region.Locals[Name];
	      set => region.Locals[Name] = value;
	   }

		public override string ContainerType => ValueType.NamespaceVariable.ToString();

	   public override string ToString() => Value.ToString();
	}
}