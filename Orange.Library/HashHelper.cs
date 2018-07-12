using System;
using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Configurations;
using Standard.ObjectGraphs;
using Standard.Types.Collections;

namespace Orange.Library
{
	public static class HashHelper
	{
		public static ObjectGraph ToGraph<TKey, TValue>(this Hash<TKey, TValue> hash, string name, Func<TKey, string> keyFunc,
			Func<string, TValue, ObjectGraph> valueFunc = null)
		{
			if (valueFunc == null)
				valueFunc = (k, v) => new ObjectGraph(k, v.ToString(), v.GetType().Name);
			var graph = new ObjectGraph(name);
			foreach (var item in hash)
			{
				var itemName = keyFunc(item.Key);
				graph[itemName] = valueFunc(itemName, item.Value);
			}
			return graph;
		}

		public static Hash<TKey, TValue> FromGraph<TKey, TValue>(this ObjectGraph graph, Func<ObjectGraph, TKey> keyFunc,
			Func<ObjectGraph, TValue> valueFunc)
		{
			var hash = new Hash<TKey, TValue>();
			foreach (var child in graph.Children)
			{
				var key = keyFunc(child);
				hash[key] = valueFunc(child);
			}
			return hash;
		}

		public static ObjectGraph ToGraph(this Hash<string, Value> hash, string name)
		{
			var graph = new ObjectGraph(name);
			foreach (var item in hash)
			{
				var valueGraph = item.Value.AsGraph(item.Key);
				graph[item.Key] = valueGraph;
			}
			return graph;
		}

		public static Hash<string, Value> FromGraph(this ObjectGraph graph)
		{
			var hash = new Hash<string, Value>();
			foreach (var child in graph.Children)
				hash[child.Name] = child.AsValue();
			return hash;
		}

		public static ObjectGraph ToGraph<T>(this Hash<string, T> hash, string name, Func<string, T, ObjectGraph> func = null)
		{
			if (func == null)
				func = (k, v) => new ObjectGraph(k, v.ToString(), v.GetType().Name);
			var graph = new ObjectGraph(name);
			foreach (var item in hash)
				graph[item.Key] = func(item.Key, item.Value);
			return graph;
		}

		public static Hash<string, T> FromGraph<T>(this ObjectGraph graph, Func<ObjectGraph, T> func)
		{
			var hash = new Hash<string, T>();
			foreach (var child in graph.Children)
				hash[child.Name] = func(child);
			return hash;
		}
	}
}