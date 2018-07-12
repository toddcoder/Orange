using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Configurations;
using Standard.Types.Objects;

namespace Orange.Library.Verbs
{
	public class Signal : Verb, ISerializeToGraph
	{
		string type;
		Block block;

		public Signal(string type, Block block)
		{
			this.type = type;
			this.block = block;
			this.block.ReturnNull = false;
		}

		public Signal()
		{
			type = "";
			block = new Block();
		}

		public override Value Evaluate()
		{
			Value value = block.Evaluate();
			if (value.IsNil)
				return null;
			switch (type)
			{
				case "return":
					Runtime.State.ReturnSignal = true;
					Runtime.State.ReturnValue = value;
					return value;
				case "exit":
					if (value.IsEmpty || value.IsTrue)
						Runtime.State.ExitSignal = true;
					return null;
				case "continue":
					if (value.IsEmpty || value.IsTrue)
						Runtime.State.SkipSignal = true;
					return null;
				case "yield":
					IStringify stringify;
					if (value.IsA(out stringify))
						value = stringify.String;
					ClosureContext context = Runtime.State.ClosureContext;
					if (context == null)
						return value;
					ArrayYielder yielder = context.Yielder;
					yielder.Add(value.Resolve());
					return value;
			}
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Push;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", type, block);
		}

		public ObjectGraph ToGraph(string name)
		{
			var graph = new ObjectGraph(name);
			graph["type"] = new ObjectGraph("type", type);
			graph["block"] = block.AsGraph("block");
			return graph;
		}

		public void FromGraph(ObjectGraph graph)
		{
			type = graph["type"].Value;
			block.FromGraph(graph["block"]);
			block.ReturnNull = false;
		}
	}
}