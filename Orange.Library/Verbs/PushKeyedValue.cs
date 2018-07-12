using System;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Verbs
{
	public class PushKeyedValue : Verb
	{
		const string LOCATION = "Push keyed value";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var value = stack.Pop(true, LOCATION);
			var keyValue = stack.Pop(false, LOCATION);
			if (keyValue.IsArray && value.IsArray)
			{
				var keys = (Array)keyValue.Resolve().SourceArray;
				var values = (Array)value.SourceArray;
				var maxLength = Math.Max(keys.Length, values.Length);
				var array = new Array();
				for (var i = 0; i < maxLength; i++)
					array[keys[i].Text] = values[i];
				return array;
			}
			var key = keyValue.Text;
			return new KeyedValue(key, value);
		}

		public override string ToString() => "=>";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.KeyedValue;
	}
}