using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class ArrayBuilder : Value
	{
		Array array;

		public ArrayBuilder(Value left, Value right) => array = new Array
		{
		   left,
		   right
		};

	   public ArrayBuilder(Array array) => this.array = array;

	   public override int Compare(Value value) => array.Compare(value);

	   public override string Text
		{
			get
			{
				return array.Text;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return array.Number;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.ArrayBuilder;

	   public override bool IsTrue => array.IsTrue;

	   public override Value Clone() => new ArrayBuilder((Array)array.Clone());

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "append", v => ((ArrayBuilder)v).Append());
		}

		public Value Append() => Append(Arguments[0]);

	   public ArrayBuilder Append(Value value)
		{
			array.Add(value);
			return this;
		}

		public override bool IsArray => true;

	   public override Value SourceArray => getArray();

	   public override Value AlternateValue(string message) => getArray();

	   public override Value AssignmentValue() => getArray();

	   public override Value ArgumentValue() => getArray();

	   public override void AssignTo(Variable variable)
		{
			array.AssignTo(variable);
		}

		public override string ToString() => array.ToString();

/*		public override Value Resolve()
		{
			return getArray();
		}*/

		Value getArray()
		{
			if (array.Length > 0 && array[0].Type == ValueType.Graph)
			{
				var graph = new Graph("", array);
				return graph;
			}
			return array;
		}
	}
}