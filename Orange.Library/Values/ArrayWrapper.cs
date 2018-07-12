using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class ArrayWrapper : Value
	{
		Array array;

		public ArrayWrapper(Array array) => this.array = array;

	   public override int Compare(Value value) => array.Compare(value);

	   public override string Text
		{
			get => array.Text;
	      set => array.Text = value;
	   }

		public override double Number
		{
			get => array.Number;
		   set => array.Number = value;
		}

		public override ValueType Type => ValueType.ArrayWrapper;

	   public override bool IsTrue => array.IsTrue;

	   public override Value Clone() => new ArrayWrapper(array);

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override Value AlternateValue(string message) => array;

	   public override Value AssignmentValue() => array;

	   public override Value ArgumentValue() => array;
	}
}