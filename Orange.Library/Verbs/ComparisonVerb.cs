using System;
using System.Linq;
using Orange.Library.Values;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Verbs
{
	public abstract class ComparisonVerb : Verb
	{
		public override Value Evaluate()
		{
			var stack = State.Stack;
			var y = stack.Pop(true, Location);
			var x = stack.Pop(true, Location);
			var exceptionValue = Exception(x, y);
			return exceptionValue ?? DoComparison(x, y);
		}

		public virtual Value Exception(Value x, Value y) => null;

	   public virtual Value DoComparison(Value x, Value y)
		{
			if (x.IsArray || y.IsArray)
				return compareArray(x, y);
			x = x.Resolve();
			y = y.Resolve();
			if (y.Type == Value.ValueType.TypeName)
				return Compare(y.Compare(x));
			var comparison = Runtime.Compare(x, y);
			return Compare(comparison);
		}

		public abstract bool Compare(int comparison);

	   public abstract string Location { get; }

		Value compareArray(Value x, Value y)
		{
			Array yArray;
			if (x.IsArray)
			{
				var xArray = (Array)x.SourceArray;
				if (y.IsArray)
				{
					yArray = (Array)y.SourceArray;
					switch (xArray.Comparison)
					{
						case Array.CompareType.Any:
							return compareArrayAny(xArray, yArray);
						case Array.CompareType.All:
							return compareArrayAll(xArray, yArray);
						case Array.CompareType.One:
							return compareArrayOne(xArray, yArray);
						case Array.CompareType.None:
							return compareArrayNone(xArray, yArray);
					}
				}
				var compareType = xArray.Comparison;
				xArray.Comparison = Array.CompareType.Any;
				switch (compareType)
				{
					case Array.CompareType.Any:
						return compareArrayAny(xArray, y);
					case Array.CompareType.All:
						return compareArrayAll(xArray, y);
					case Array.CompareType.One:
						return compareArrayOne(xArray, y);
					case Array.CompareType.None:
						return compareArrayNone(xArray, y);
				}
				return compareArrayAny(xArray, y);
			}
			if (y.IsArray)
			{
				yArray = (Array)y.SourceArray;
				var compareType = yArray.Comparison;
				yArray.Comparison = Array.CompareType.Any;
				switch (compareType)
				{
					case Array.CompareType.Any:
						return compareArrayAny(x, yArray);
					case Array.CompareType.All:
						return compareArrayAll(x, yArray);
					case Array.CompareType.One:
						return compareArrayOne(x, yArray);
					case Array.CompareType.None:
						return compareArrayNone(x, yArray);
				}
				return compareArrayAny(x, yArray);
			}
			return Compare(x.Compare(y));
		}

		bool compareArrayAny(Array x, Value y) => x.Any(i => Compare(i.Value.Compare(y)));

	   bool compareArrayAny(Value x, Array y) => y.Any(i => Compare(x.Compare(i.Value)));

	   bool compareArrayAny(Array x, Array y)
		{
			var length = Math.Min(x.Length, y.Length);
			for (var i = 0; i < length; i++)
				if (Compare(x[i].Compare(y[i])))
					return true;
			return false;
		}

		bool compareArrayAll(Array x, Value y) => x.All(i => Compare(i.Value.Compare(y)));

	   bool compareArrayAll(Value x, Array y) => y.All(i => Compare(x.Compare(i.Value)));

	   bool compareArrayAll(Array x, Array y)
		{
			var length = Math.Min(x.Length, y.Length);
			for (var i = 0; i < length; i++)
				if (!Compare(x[i].Compare(y[i])))
					return false;
			return true;
		}

		bool compareArrayOne(Array x, Value y)
		{
			var found = false;
			foreach (var _ in x.Where(item => Compare(item.Value.Compare(y))))
			{
				if (found)
					return false;
				found = true;
			}
			return found;
		}

		bool compareArrayOne(Value x, Array y)
		{
			var found = false;
			foreach (var _ in y.Where(item => Compare(x.Compare(item.Value))))
			{
				if (found)
					return false;
				found = true;
			}
			return found;
		}

		bool compareArrayOne(Array x, Array y)
		{
			var found = false;
			var length = Math.Min(x.Length, y.Length);
			for (var i = 0; i < length; i++)
			{
				if (!Compare(x[i].Compare(y[i])))
					continue;
				if (found)
					return false;
				found = true;
			}
			return found;
		}

		bool compareArrayNone(Array x, Value y) => !x.Any(i => Compare(i.Value.Compare(y)));

	   bool compareArrayNone(Value x, Array y) => !y.Any(i => Compare(x.Compare(i.Value)));

	   bool compareArrayNone(Array x, Array y)
		{
			var length = Math.Min(x.Length, y.Length);
			for (var i = 0; i < length; i++)
				if (Compare(x[i].Compare(y[i])))
					return false;
			return true;
		}
	}
}