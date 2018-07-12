using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class BlockPattern : Value
	{
		BlockMatcher matcher;

		public BlockPattern(Block input, Block pattern) => matcher = new BlockMatcher
		{
		   Input = input,
		   Pattern = pattern
		};

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
			get;
			set;
		}

		public override ValueType Type => ValueType.BlockPattern;

	   public override bool IsTrue => false;

	   public override Value Clone() => null;

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "replace", v => ((BlockPattern)v).Replace());
		}

		public Block Replacment
		{
			get => matcher.Replacement;
		   set => matcher.Replacement = value;
		}

		public Value Replace() => matcher.Replace();
	}
}