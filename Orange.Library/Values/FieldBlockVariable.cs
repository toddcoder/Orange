using Standard.Configurations;

namespace Orange.Library.Values
{
	public class FieldBlockVariable : FieldVariable
	{
		Block block;

		public FieldBlockVariable(Block block)
			: base(-1)
		{
			this.block = block;
			this.block.ReturnNull = false;
		}

		public FieldBlockVariable()
			: this(new Block())
		{
		}

		public override Value Value
		{
			get
			{
				if (!Runtime.State.VariableExists(Runtime.VAR_NF))
				{
					index = 1;
					getValue();
				}
				index = (int)block.Evaluate().Number;
				return getValue();
			}
			set
			{
				if (!Runtime.State.VariableExists(Runtime.VAR_NF))
				{
					index = 1;
					setValue(value);
				}
				index = (int)block.Evaluate().Number;
				base.Value = value;
			}
		}

		public override string ToString()
		{
			return string.Format("$({0})", block);
		}

		public override ObjectGraph ToGraph(string name)
		{
			var graph = new ObjectGraph(name, subName: "FieldBlockVariable");
			graph["name"] = new ObjectGraph("name", Name);
			graph["block"] = block.ToGraph("block");
			return graph;
		}

		public override void FromGraph(ObjectGraph graph)
		{
			ObjectGraph nameGraph = graph["name"];
			ObjectGraph blockGraph = graph["block"];
			Name = nameGraph.Value;
			block = new Block();
			block.FromGraph(blockGraph);
			block.ReturnNull = false;
		}
	}
}