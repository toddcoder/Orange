using Orange.Library.Managers;
using Standard.Types.Exceptions;

namespace Orange.Library.Values
{
	public class ToDo : Value
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

		public override ValueType Type => ValueType.ToDo;

	   public override bool IsTrue => false;

	   public override Value Clone() => new ToDo();

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "invoke", v => ((ToDo)v).Invoke());
		}

		public Value Invoke() => throw "Can't invoke a todo".Throws();
	}
}