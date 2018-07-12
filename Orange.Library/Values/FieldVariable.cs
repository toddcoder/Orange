using Standard.Configurations;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
	public class FieldVariable : Variable
	{
		protected int index;

		public FieldVariable(int index)
			: base("$" + index)
		{
			this.index = index;
		}

		public FieldVariable()
			: base("$unset")
		{
			index = -1;
		}

		public override Value Value
		{
			get
			{
				return getValue();
			}
			set
			{
				setValue(value);
			}
		}

		protected void setValue(Value value)
		{
			if (index == 0)
				Runtime.State.SetLocal(Runtime.VAR_FIELDS, value.Text);
			else
			{
				if (value.Type == ValueType.Nil)
					return;
				Value fields = Runtime.State[Runtime.VAR_FIELDS];
				fields = arrayify(fields);
				var array = (Array)fields;
				array[index - 1] = value.Text;
				Runtime.State.SetLocal(Runtime.VAR_NF, array.Length);
			}
		}

		protected Value getValue()
		{
			Value fields = Runtime.State[Runtime.VAR_FIELDS];
			if (index == 0)
				return Runtime.ConvertIfNumeric(fields.Text);
			fields = arrayify(fields);
			return Runtime.ConvertIfNumeric(((Array)fields)[index - 1].Text);
		}

		Value arrayify(Value fields)
		{
			if (!fields.IsArray)
			{
				string[] values = Runtime.State.FieldPattern.Split(fields.Text);
				int length = values.Length;
				fields = new Array(values);
				Runtime.State.SetLocal(Runtime.VAR_FIELDS, fields);
				Runtime.State.SetLocal(Runtime.VAR_NF, length);
			}
			return fields;
		}

		public override string ContainerType
		{
			get
			{
				return ValueType.FieldVariable.ToString();
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public override ObjectGraph ToGraph(string name)
		{
			var graph = new ObjectGraph(name, subName: "FieldVariable");
			graph["name"] = new ObjectGraph("name", Name);
			graph["index"] = new ObjectGraph("index", index.ToString());
			return graph;
		}

		public override void FromGraph(ObjectGraph graph)
		{
			ObjectGraph nameGraph = graph["name"];
			ObjectGraph indexGraph = graph["index"];
			Name = nameGraph.Value;
			index = indexGraph.Value.ToInt();
		}
	}
}