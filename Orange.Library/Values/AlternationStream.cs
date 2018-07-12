using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class AlternationStream : Value, ISequenceSource
	{
		Alternation alternation;
		int limit;

		public AlternationStream(Alternation alternation)
		{
			this.alternation = alternation;
			limit = Runtime.MAX_ARRAY;
		}

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

		public override ValueType Type => ValueType.AlternationStream;

	   public override bool IsTrue => false;

	   public override Value Clone() => new AlternationStream((Alternation)alternation.Clone());

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "map", v => ((AlternationStream)v).Map());
			manager.RegisterMessage(this, "if", v => ((AlternationStream)v).If());
			manager.RegisterMessage(this, "unless", v => ((AlternationStream)v).Unless());
			manager.RegisterMessage(this, "take", v => ((AlternationStream)v).Take());
			manager.RegisterMessage(this, "next", v => ((AlternationStream)v).Next());
			manager.RegisterMessage(this, "reset", v => ((AlternationStream)v).Reset());
		}

		public Value Map() => new Sequence(this)
		{
		   Arguments = Arguments.Clone()
		}.Map();

	   public Value If() => new Sequence(this)
	   {
	      Arguments = Arguments.Clone()
	   }.If();

	   public Value Unless() => new Sequence(this)
	   {
	      Arguments = Arguments.Clone()
	   }.Unless();

	   public Value Take() => new Sequence(this)
	   {
	      Arguments = Arguments.Clone()
	   }.Take();

	   public Value Next()
		{
			var value = alternation.Dequeue();
			if (value.IsNil)
			{
				alternation.Reset();
				value = alternation.Dequeue();
			}
			return value;
		}

		public ISequenceSource Copy() => (ISequenceSource)Clone();

	   public Value Reset()
		{
			alternation.Reset();
			return this;
		}

		public int Limit
		{
			get => limit;
		   set => limit = value < 0 ? Runtime.MAX_ARRAY : value;
		}

		public Array Array => Array.ArrayFromSequence(this);
	}
}