using System.Collections.Generic;
using Orange.Library.Managers;
using System.IO;
using Orange.Library.Templates;
using Standard.Configurations;
using System.Linq;

namespace Orange.Library.Values
{
	public class Template : Value
	{
		Item[] items;
		string variableName;

		public Template(Item[] items)
		{
			this.items = items;
			variableName = Runtime.VAR_ANONYMOUS + Compiler.CompilerState.ObjectID();
		}

		public Template()
			: this(new Item[0])
		{
		}

		public override int Compare(Value value)
		{
			return 0;
		}

		public override string Text
		{
			get
			{
				Stack<string> stack = null;
				using (var writer = new StringWriter())
				{
					writer.WriteLine("{0} = '';", variableName);
					foreach (Item item in items)
					{
						var xmlStack = item as IXMLStack;
						if (xmlStack != null)
						{
							if (stack == null)
								stack = new Stack<string>();
							xmlStack.Stack = stack;
						}
						item.Render(writer, variableName);
					}
					while (stack != null && stack.Count > 0)
						writer.WriteLine("|</{0}>|.print('{1}');", stack.Pop(), variableName);

					string code = writer.ToString();
					return code;
				}
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

		public override ValueType Type
		{
			get
			{
				return ValueType.Template;
			}
		}

		public override bool IsTrue
		{
			get
			{
				return false;
			}
		}

		public override Value Clone()
		{
			return null;
		}

		protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "eval", v => ((Template)v).Evaluate());
		}

		public Value Evaluate()
		{
			string source = Text;
			var compiler = new OrangeCompiler(source.Trim());
			Block block = compiler.Compile();
			block.Evaluate();
			return RegionManager.Regions[variableName].Text;
		}

		public override string ToString()
		{
			return Text;
		}

		public ObjectGraph ToGraph(string name)
		{
			var graph = new ObjectGraph(name, type: "Template");
			var itemsGraph = new ObjectGraph("items");
			var index = new GraphIndexer();
			foreach (Item item in items)
			{
				string key = index.ToString();
				itemsGraph[key] = item.ToGraph(key);
			}
			graph["items"] = itemsGraph;
			graph["var"] = new ObjectGraph("var", variableName);
			return graph;
		}

		public void FromGraph(ObjectGraph graph)
		{
			items = graph["items"].Children.Select(Item.ItemFromGraph).ToArray();
			variableName = graph["var"].Value;
		}
	}
}