using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class AutoInvoker : Value
	{
		IInvokeable invokeable;
		Value invokeableAsValue;

		public AutoInvoker(IInvokeable invokeable)
		{
			this.invokeable = invokeable;
			invokeableAsValue = (Value)this.invokeable;
		}

		public override int Compare(Value value) => invokeableAsValue.Compare(value);

	   public override string Text
		{
			get
			{
				return invokeableAsValue.Text;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return invokeableAsValue.Number;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.AutoInvoker;

	   public override bool IsTrue => invokeableAsValue.IsTrue;

	   public override Value Clone() => new AutoInvoker(invokeable);

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override Value Resolve() => invoke();

	   public override Value ArgumentValue() => invoke();

	   public override Value AssignmentValue() => invoke();

	   Value invoke() => invokeable.Invoke(new Arguments());
	}
}