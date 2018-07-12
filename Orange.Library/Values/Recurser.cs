using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Recurser : Value
	{
		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.Recurser;

	   public override bool IsTrue => false;

	   public override Value Clone() => new Recurser();

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "invoke", v => ((Recurser)v).Invoke());
		}

		public Value Invoke()
		{
			var values = Arguments.Values;
			for (var i = 0; i < values.Length; i++)
				RegionManager.Regions.SetLocal(Arguments.VariableName(i), values[i]);
			return null;
		}
	}
}