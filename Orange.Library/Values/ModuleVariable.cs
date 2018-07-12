namespace Orange.Library.Values
{
	public class ModuleVariable : Variable
	{
		Module module;

		public ModuleVariable(string name, Module module)
			: base(name) => this.module = module;

	   public override Value Value
		{
			get => module[Name];
		   set => module[Name] = value;
		}

		public override string ContainerType => ValueType.ModuleVariable.ToString();
	}
}