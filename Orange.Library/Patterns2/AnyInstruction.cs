namespace Orange.Library.Patterns2
{
	public class AnyInstruction : Instruction
	{
		CharSet charSet;

		public AnyInstruction(string source) => charSet = new CharSet(source);

	   public override bool Execute(State state)
		{
			if (charSet.Contains(state.CurrentCharacter, state.IgnoreCase))
			{
				state.Position++;
				state.InstructionIndex++;
				return true;
			}
			state.InstructionIndex++;
			return false;
		}
	}
}