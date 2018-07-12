using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Binding : Value
	{
		Region region;

		public Binding(Region region) => this.region = region;

	   public Binding()
		{
			region = new Region();
			RegionManager.Regions.Current.CopyAllVariablesTo(region);
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get;
			set;
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.Binding;

	   public override bool IsTrue => false;

	   public override Value Clone() => new Binding(region);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "use", v => ((Binding)v).Use());
		}

		public Value Use()
		{
			var block = Arguments.Executable;
			if (block.CanExecute)
			{
				block.AutoRegister = false;
				Runtime.State.RegisterBlock(block, region);
				block.Evaluate();
				Runtime.State.UnregisterBlock();
			}
			else
				foreach (var item in region.Locals)
					RegionManager.Regions[item.Key] = item.Value;
			return this;
		}
	}
}