using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Strings;
using static System.Math;

namespace Orange.Library.Values
{
	public class TypeName : Value
	{
		string name;

		public TypeName(string name) => this.name = name;

	   public override int Compare(Value value)
		{
			string typeName;
			switch (value.Type)
			{
				case ValueType.TypeName:
					return ((TypeName)value).name == name ? 0 : 1;
				default:
					typeName = value.Type.ToString();
					break;
			}
			switch (name)
			{
			   case "Int":
			      return value.Type == ValueType.Number && (int)Truncate(value.Number) == (int)value.Number ? 0 : 1;
			   case "Float":
			      return value.Type == ValueType.Number && (int)Truncate(value.Number) != (int)value.Number ? 0 : -1;
			   case "Scalar":
			      return value.IsArray ? 1 : 0;
            case "Hash":
			      return value.IsArray && !((Array)value.SourceArray).Keys.Any(Array.IsAutoAssignedKey) ? 0 : 1;
			}
		   return string.CompareOrdinal(name, typeName);
		}

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

		public override ValueType Type => ValueType.TypeName;

	   public override bool IsTrue => false;

	   public override Value Clone() => new TypeName(name.Copy());

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "apply", v => ((TypeName)v).Apply());
		}

		public override string ToString() => $"#{name}";

	   public Value Apply()
		{
			var value = Arguments.ApplyValue;
			return Compare(value) == 0 ? value : new Nil();
		}
	}
}