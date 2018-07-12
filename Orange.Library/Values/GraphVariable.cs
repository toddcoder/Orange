namespace Orange.Library.Values
{
	public class GraphVariable : Variable
	{
		ValueGraph graph;
		ValueGraph parent;

		public GraphVariable(string name, ValueGraph graph, ValueGraph parent = null)
			: base(name)
		{
			this.graph = graph;
			this.parent = parent;
		}

		public override Value Value
		{
			get => graph.Value;
		   set
			{
				if (value.IsNil && parent != null)
				{
					parent.Remove(graph.Name);
					return;
				}
				graph.Value = value;
				if (parent != null)
					parent[Name] = graph;
			}
		}

		public override string ContainerType => ValueType.GraphVariable.ToString();
	}
}