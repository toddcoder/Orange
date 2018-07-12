using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class CaseReplacement : Value
	{
		Object obj;
		MessagePath messagePath;
		Namespace ns;

		public CaseReplacement(Object obj, MessagePath messagePath)
		{
			this.obj = obj;
			this.messagePath = messagePath;
			ns = new Namespace();
			Runtime.State.CurrentNamespace.CopyAllVariables(ns);
		}

		public override int Compare(Value value)
		{
			return getValue().Compare(value);
		}

		public override string Text
		{
			get
			{
				return getValue().Text;
			}
			set
			{
				getValue().Text = value;
			}
		}

		public override double Number
		{
			get
			{
				return getValue().Number;
			}
			set
			{
				getValue().Number = value;
			}
		}

		public override ValueType Type
		{
			get
			{
				return ValueType.CaseReplacement;
			}
		}

		public override bool IsTrue
		{
			get
			{
				return getValue().IsTrue;
			}
		}

		public override Value Clone()
		{
			return new CaseReplacement((Object)obj.Clone(), (MessagePath)messagePath.Clone());
		}

		protected override void registerMessages(MessageManager manager)
		{
		}

		public override Value AlternateValue(string message)
		{
			return getValue();
		}

		Value getValue()
		{
			Value invoke = messagePath.Invoke(obj);
			return invoke;
		}

		public override Value AssignmentValue()
		{
			return getValue();
		}

		public override Value ArgumentValue()
		{
			return getValue();
		}

		public override string ToString()
		{
			return getValue().ToString();
		}
	}
}