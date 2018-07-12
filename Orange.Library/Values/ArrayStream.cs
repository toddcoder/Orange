using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class ArrayStream : Value, ISequenceSource
	{
		static ParameterBlock defaultIf()
		{
			var builder = new CodeBuilder();
			builder.Value(true);
			var block = builder.Block;
			return new ParameterBlock(block);
		}

		protected Value seed;
		ParameterBlock next;
		protected int limit;
		protected ParameterBlock ifBlock;
		Value current;
		int count;
		protected Region region;

		public ArrayStream(Value seed, ParameterBlock next, Region region = null)
		{
			this.seed = seed;
			this.next = next;
			this.region = Region.CopyCurrent(region);
			limit = Runtime.MAX_ARRAY;
			ifBlock = defaultIf();
			current = null;
			count = 0;
		}

		public CFor CFor(ParameterBlock incrementBlock) => new CFor(seed, next, incrementBlock);

	   public virtual int Limit
		{
			get => limit;
	      set => limit = value < 0 ? Runtime.MAX_ARRAY : value;
	   }

		public Array Array => (Array)AlternateValue("");

	   public override int Compare(Value value) => 0;

	   public override string Text
		{
			get;
			set;
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.ArrayStream;

	   public override bool IsTrue => false;

	   public override Value Clone() => new ArrayStream(seed.Clone(), next.Clone())
	   {
	      ifBlock = ifBlock.Clone(),
	      limit = limit
	   };

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "map", v => ((ArrayStream)v).Map());
			manager.RegisterMessage(this, "if", v => ((ArrayStream)v).IfMessage());
			manager.RegisterMessage(this, "unless", v => ((ArrayStream)v).Unless());
			manager.RegisterMessage(this, "take", v => ((ArrayStream)v).Take());
			manager.RegisterMessage(this, "next", v => ((ArrayStream)v).Next());
			manager.RegisterMessage(this, "reset", v => ((ArrayStream)v).Reset());
		}

		public override Value AlternateValue(string message)
		{
			using (var popper = new RegionPopper(region, "array-stream-alt"))
			{
				popper.Push();
				var array = new Array();
				if (evaluate(ifBlock, seed).IsTrue)
				{
					array.Add(seed);
					var value = seed;
					while (array.Length < limit)
					{
						value = evaluate(next, value);
						if (!evaluate(ifBlock, value).IsTrue)
							break;
						array.Add(value);
					}
				}
				return array;
			}
		}

		public virtual Value Next()
		{
			using (var popper = new RegionPopper(region, "array-stream-next"))
			{
				popper.Push();
				if (current == null)
				{
					if (evaluate(ifBlock, seed).IsTrue)
					{
						current = seed;
						count++;
						return seed;
					}
				}
				else if (count < limit)
				{
					current = evaluate(next, current);
					if (evaluate(ifBlock, current).IsTrue)
					{
						count++;
						return current;
					}
					return new Nil();
				}
				return new Nil();
			}
		}

		public ISequenceSource Copy() => (ISequenceSource)Clone();

	   protected static Value evaluate(ParameterBlock parameterBlock, Value current)
		{
			if (parameterBlock == null)
				return true;
			using (var assistant = new ParameterAssistant(parameterBlock))
			{
				var block = assistant.Block();
				if (block == null)
					return true;
				assistant.IteratorParameter();
				assistant.SetIteratorParameter(current);
				return block.Evaluate();
			}
		}

		public virtual Value Reset()
		{
			current = null;
			count = 0;
			return this;
		}

		public override bool IsArray => false;

	   public override Value SourceArray => AlternateValue("");

	   public virtual Value Map()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.Map();
		}

		public virtual Value IfMessage()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.If();
		}

		public virtual Value Unless()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.Unless();
		}

		public virtual Value Take()
		{
			var sequence = new Sequence(this)
			{
				Arguments = Arguments.Clone()
			};
			return sequence.Take();
		}

		public override string ToString() => seed + " to " + next + " by " + limit;
	}
}