using Orange.Library.Values;

namespace Orange.Library.Patterns2
{
	class LengthBlockInstruction : LengthInstruction
	{
		Block countBlock;

		public LengthBlockInstruction(Block countBlock)
			: base(0) => this.countBlock = countBlock;

	   public override void Initialize()
		{
			base.Initialize();
			count = getCount();
		}

		int getCount() => (int)countBlock.Evaluate().Number;
	}
}