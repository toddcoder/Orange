using System.Collections.Generic;
using Orange.Library.Values;
using System.Linq;
using Standard.Types.Strings;

namespace Orange.Library
{
	public class ArrayPipeline
	{
		public abstract class Item
		{
			public abstract Value Process(Array.IterItem item, int length);

			public virtual bool IsTerminating
			{
				get
				{
					return false;
				}
			}
		}

		public class Map : Item
		{
			ParameterBlock parameterBlock;

			public Map(ParameterBlock parameterBlock)
			{
				this.parameterBlock = parameterBlock;
			}

			public override Value Process(Array.IterItem item, int length)
			{
				using (var assistant = new ParameterAssistant(parameterBlock))
				{
					Block block = assistant.Block();
					assistant.ArrayParameters();
					assistant.SetParameterValues(item);
					return block.Evaluate();
				}
			}

			public override string ToString()
			{
				return "-> " + parameterBlock;
			}
		}

		public class If : Item
		{
			ParameterBlock parameterBlock;

			public If(ParameterBlock parameterBlock)
			{
				this.parameterBlock = parameterBlock;
			}

			public override Value Process(Array.IterItem item, int length)
			{
				using (var assistant = new ParameterAssistant(parameterBlock))
				{
					Block block = assistant.Block();
					assistant.ArrayParameters();
					assistant.SetParameterValues(item);
					return block.Evaluate().IsTrue ? item.Value : null;
				}
			}

			public override string ToString()
			{
				return "-? " + parameterBlock;
			}
		}

		public class Take : Item
		{
			int limit;

			public Take(int limit)
			{
				this.limit = limit;
			}

			public override Value Process(Array.IterItem item, int length)
			{
				return length < limit ? item.Value : null;
			}

			public override bool IsTerminating
			{
				get
				{
					return true;
				}
			}

			public override string ToString()
			{
				return @"\\ " + limit;
			}
		}

		List<Item> items;

		public ArrayPipeline()
		{
			items = new List<Item>();
		}

		public void Add(Item item)
		{
			items.Add(item);
		}

		public Value Evaluate(Value source)
		{
			switch (source.Type)
			{
				case Value.ValueType.ArrayStream:
					return Evaluate((ArrayStream)source);
				case Value.ValueType.ArrayYielder:
					return Evaluate((ArrayYielder)source);
				default:
					return Evaluate((Array)source.SourceArray);
			}
		}

		public Value Evaluate(Array array)
		{
			var newArray = new Array();
			foreach (Array.IterItem item in array)
			{
				Value value = item.Value;
				var skip = false;
				foreach (Item pipeItem in items)
				{
					value = pipeItem.Process(item, newArray.Length);
					if (value == null || value.Type == Value.ValueType.Nil)
					{
/*						if (pipeItem.IsTerminating)
							return newArray;*/
						skip = true;
						break;
					}
				}
				if (!skip)
					newArray.Add(value);
			}
			return newArray;
		}

		public Value Evaluate(ArrayStream stream)
		{
			var array = new Array();
			while (array.Length < stream.Limit)
			{
				Value value = stream.Next();
				if (value.Type == Value.ValueType.Nil)
					return array;
				var skip = false;
				foreach (Item item in items)
				{
					var iterItem = new Array.IterItem
					{
						Value = value,
						Key = Array.GetKey(),
						Index = array.Length
					};
					value = item.Process(iterItem, array.Length);
					if (value == null || value.Type == Value.ValueType.Nil)
					{
/*						if (item.IsTerminating)
							return array;*/
						skip = true;
						break;
					}
				}
				if (!skip)
					array.Add(value);
			}
			return array;
		}

		public Value Evaluate(ArrayYielder yielder)
		{
			var array = new Array();
			foreach (Closure closure in yielder.Items)
			{
				Value value = closure.Evaluate(new Arguments());
				if (value.Type == Value.ValueType.Nil)
					return array;
				var skip = false;
				foreach (Item item in items)
				{
					var iterItem = new Array.IterItem
					{
						Value = value,
						Key = Array.GetKey(),
						Index = array.Length
					};
					value = item.Process(iterItem, array.Length);
					if (value == null || value.Type == Value.ValueType.Nil)
					{
						skip = true;
						break;
					}
				}
				if (!skip)
					array.Add(value);
			}
			return array;
		}

		public override string ToString()
		{
			return items.Select(i => i.ToString()).Listify(" ");
		}
	}
}