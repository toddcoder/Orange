using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Placeholder : Value
	{
		string name;

		public Placeholder(string name) => this.name = name;

	   public override int Compare(Value value) => string.CompareOrdinal(name, value.Text);

	   public override string Text
		{
			get
			{
				return name;
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

		public override ValueType Type => ValueType.Placeholder;

	   public override bool IsTrue => true;

	   public override Value Clone() => new Placeholder(name);

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override string ToString() => $"*{name}";
	}
}