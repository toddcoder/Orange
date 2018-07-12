using Orange.Library.Values;

namespace Orange.Library
{
	public class BlockState
	{
		Block block;
		VerbStack verbStack;
		ValueStack valueStack;
		int index;

		public BlockState(Block block)
		{
			this.block = block;
			if (this.block.AutoRegister)
				Runtime.State.RegisterBlock(this.block, this.block.ResolveVariables);
			verbStack = Runtime.State.Expressions.Current;
			valueStack = Runtime.State.Stack;
			Stringify = null;
			index = -1;
		}

		public VerbStack VerbStack => verbStack;

	   public ValueStack ValueStack => valueStack;

	   public IStringify Stringify
		{
			get;
			set;
		}

		public int Index => index;

	   public bool Next() => index++ < block.Count;

	   public Value ReturnValue
		{
			get;
			set;
		}
	}
}