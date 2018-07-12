using System.Linq;
using Orange.Library.Managers;
using Orange.Library.Values;
using static System.Math;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library
{
	public class ParallelZip : Value, ISequenceSource
	{
		const string LOCATION = "Parallel zip";

		Array arrayOfArrays;
		int index;
		int maxLength;

		public ParallelZip(Array arrayOfArrays)
		{
			this.arrayOfArrays = arrayOfArrays;
			index = -1;
			maxLength = -1;
			foreach (var value in this.arrayOfArrays.Select(item => item.Value))
			{
				Assert(value.IsArray, LOCATION, $"Item {value} in array of arrays not an array");
				var array = (Array)value.SourceArray;
				maxLength = Max(maxLength, array.Length);
			}
		}

		public Value Next()
		{
			if (++index >= maxLength)
				return new Nil();
			var newArray = new Array();
			foreach (var array in arrayOfArrays.Select(item => (Array)item.Value))
				newArray.Add(array[index]);
			return newArray;
		}

		public ISequenceSource Copy() => new ParallelZip((Array)arrayOfArrays.Clone());

	   public Value Reset()
		{
			index = -1;
			return this;
		}

		public int Limit => MAX_ARRAY;

	   public Array Array => Array.ArrayFromSequence(this);

	   public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.ParallelZip;

	   public override bool IsTrue => arrayOfArrays.Length > 0;

	   public override Value Clone() => (Value)Copy();

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "map", v => ((ParallelZip)v).Map());
			manager.RegisterMessage(this, "if", v => ((ParallelZip)v).If());
			manager.RegisterMessage(this, "unless", v => ((ParallelZip)v).Unless());
			manager.RegisterMessage(this, "take", v => ((ParallelZip)v).Take());
			manager.RegisterMessage(this, "next", v => ((ParallelZip)v).Next());
			manager.RegisterMessage(this, "reset", v => ((ParallelZip)v).Reset());
		}

		public Value Map()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.Map();
		}

		public Value If()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.If();
		}

		public Value Unless()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.Unless();
		}

		public Value Take()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.Take();
		}

		public override string ToString() => "parallel zip";

	   public override Value AlternateValue(string message) => Array;
	}
}