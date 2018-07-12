using System;
using System.Collections.Generic;
using System.Linq;
using Standard.ObjectGraphs;

namespace Orange.Library
{
	public static class EnumerationHelper
	{
		 public static ObjectGraph EnumToGraph<T>(this IEnumerable<T> enumerable, string name, Func<T, string> keyFunc = null,
			 Func<string, T, ObjectGraph> valueFunc = null)
		 {
			 var index = new GraphIndexer();
			 if (keyFunc == null)
				 keyFunc = v => index.ToString();
			 if (valueFunc == null)
				 valueFunc = (k, v) => new ObjectGraph(k, v.ToString(), v.GetType().Name);
			 var graph = new ObjectGraph(name, type: $"IEnumerable<{typeof(T).Name}>");
			 foreach (var value in enumerable)
			 {
				 var key = keyFunc(value);
				 graph[key] = valueFunc(key, value);
			 }
			 return graph;
		 }

		public static IEnumerable<T> EnumFromGraph<T>(this ObjectGraph graph, Func<ObjectGraph, T> func) => graph.Children.Select(func);
	}
}