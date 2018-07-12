using System;
using Orange.Library.Values;
using Standard.Configurations;
using Standard.ObjectGraphs;
using Standard.Types.Objects;

namespace Orange.Library
{
	public static class ValueHelper
	{
		public static ObjectGraph AsGraph(this Value value, string name)
		{
		   return value.As<ISerializeToGraph>().Map(s => s.ToGraph(name),
            () => new ObjectGraph(name, type: value.GetType().Name));
		}

		public static Value AsValue(this ObjectGraph graph)
		{
			var type = Type.GetType(graph.Type);
			var value = (Value)Activator.CreateInstance(type);
			value.Do<Value, ISerializeToGraph>(i => i.FromGraph(graph));
			return value;
		}
	}
}