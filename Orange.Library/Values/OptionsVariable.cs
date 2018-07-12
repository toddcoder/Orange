using System.Linq;

namespace Orange.Library.Values
{
	public class OptionsVariable : Variable
	{
		Value value;

		public OptionsVariable(Value value)
			: base("")
		{
			this.value = value;
		}

		public override Value Value
		{
			get
			{
				return value.Options == null ? (Value)"" : new Array(value.Options);
			}
			set
			{
				if (value.IsArray)
					this.value.SetOptions(((Array)value.SourceArray).Values.Select(v => v.Text).ToArray());
				else
					this.value.SetOption(value.Text);
			}
		}

		public override string ContainerType
		{
			get
			{
				return ValueType.OptionsVariable.ToString().ToLower();
			}
		}
	}
}