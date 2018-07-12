using System;
using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Types.Collections;
using System.Linq;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Orange.Library
{
	public class Memoizer
	{
		Hash<string, Value> memo;
		string key;

		public Memoizer()
		{
			memo = new Hash<string, Value>();
			key = "";
		}

		public void Evaluate(IEnumerable<ParameterValue> parameterValues) => key = parameterValues
         .Select(v => v.Value.ToString()).Listify("¶");

	   public Value Evaluate(Func<Value> func)
		{
			if (memo.ContainsKey(key))
				return memo[key];
			var originalKey = key.Copy();
			var result = func();
			memo[originalKey] = result;
			return result;
		}
	}
}