namespace Orange.Library.Patterns2
{
	public class FailedInstruction : Instruction
	{
		public override bool Execute(State state) => false;
	}
}