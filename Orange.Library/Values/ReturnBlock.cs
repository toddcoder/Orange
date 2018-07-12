namespace Orange.Library.Values
{
	public class ReturnBlock : Block
	{
		public ReturnBlock(Block block)
		{
			foreach (var verb in block.AsAdded)
				Add(verb);
			AutoRegister = block.AutoRegister;
		}

		public override Value Evaluate()
		{
			var result = base.Evaluate();
			if (Runtime.State.ReturnSignal)
			{
				result = Runtime.State.ReturnValue;
				Runtime.State.ReturnSignal = false;
			}
			return result;
		}
	}
}