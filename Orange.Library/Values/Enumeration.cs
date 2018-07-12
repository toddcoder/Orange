using System.Linq;
using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class Enumeration : Value, IMessageHandler
	{
		string name;
		Hash<string, int> values;

		public Enumeration()
		{
			name = "$unknown";
			values = new Hash<string, int>();
		}

		public Enumeration(Hash<string, int> values)
			: this()
		{
			foreach (var item in values)
				this.values[item.Key] = item.Value;
		}

		public void Add(string memberName, int value) => values[memberName] = value;

	   public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return values.KeyArray().Listify(State.FieldSeparator.Text);
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Enumeration;

	   public override bool IsTrue => true;

	   public override Value Clone() => new Enumeration(values);

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
		{
			if (RespondsTo(messageName))
			{
				handled = true;
				return new EnumerationItem(name, messageName, values[messageName]);
			}
			handled = false;
			return null;
		}

		public bool RespondsTo(string messageName) => values.ContainsKey(messageName);

	   public override Value AlternateValue(string message)
		{
			var array = new Array();
			foreach (var item in values)
				array[item.Key] = item.Value;
			return array;
		}

		public override bool IsArray => true;

	   public override Value SourceArray => AlternateValue("source array");

	   public override string ToString() => values.Select(i => $"{i.Key} => {i.Value}").Listify(State.FieldSeparator.Text);

	   public override void AssignTo(Variable variable)
		{
			name = variable.Name;
			base.AssignTo(variable);
		}
	}
}