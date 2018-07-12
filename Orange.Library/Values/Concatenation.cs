using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;

namespace Orange.Library.Values
{
	public class Concatenation : Value
	{
		List<Value> values;

		public Concatenation() => values = new List<Value>();

	   public Concatenation(List<Value> values) => this.values = values;

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

		public override ValueType Type => ValueType.Concatenation;

	   public override bool IsTrue => false;

	   public override Value Clone() => new Concatenation(values.Select(v => v.Clone()).ToList());

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public void Add(Value value) => values.Add(value);

	   public int Length => values.Count;

	   public Value this[int index]
		{
			get
			{
				if (index < 0 || index >= values.Count)
					return "";
				return values[index];
			}
		}

		public override string ToString() => values.Listify(" && ");
	}
}