namespace Orange.Library.Patterns2
{
	public class SpanInstruction : Instruction
	{
		CharSet charSet;

		public SpanInstruction(string source) => charSet = new CharSet(source);

	   public override bool Execute(State state)
		{
			if (charSet.Contains(state.CurrentCharacter, state.IgnoreCase))
			{
				state.Position++;
				return true;
			}
			state.InstructionIndex++;
			return true;
		}

		public override bool ExecuteFirst(State state)
		{
			var index = charSet.IndexOf(state.CurrentInput, state.IgnoreCase);
			if (index > -1)
				state.Position += index;
			state.InstructionIndex++;
			return true;
		}
	}
}