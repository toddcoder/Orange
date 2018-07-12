using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;

namespace Orange.Library.Values
{
	public class Alternation : Value
	{
		List<Value> alternations;
		int index;

		public Alternation()
		{
			alternations = new List<Value>();
			index = -1;
		}

		public Alternation(List<Value> alternations, int index = -1)
		{
			this.alternations = alternations;
			this.index = index;
		}

		public override int Compare(Value value) => Dequeue().Compare(value);

	   public override string Text
		{
			get
			{
				return Dequeue().Text;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return Dequeue().Number;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Alternation;

	   public override bool IsTrue => alternations.Count > 0 && index < alternations.Count;

	   public override Value Clone() => new Alternation(alternations, index);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "enq", v => ((Alternation)v).Enqueue());
			manager.RegisterMessage(this, "deq", v => ((Alternation)v).Dequeue());
			manager.RegisterMessage(this, "reset", v => ((Alternation)v).Reset());
		}

		public override string ToString() => alternations.Select(v => v.ToString()).Listify(" | ");

	   public void Add(Value value) => alternations.Add(value);

	   public Value Enqueue()
		{
			var value = Arguments[0];
			Add(value);
			return this;
		}

		public Value Dequeue() => ++index < alternations.Count ? alternations[index] : new Nil();

	   public Value Reset()
		{
			index = -1;
			return this;
		}
	}
}