namespace Orange.Library.Patterns2
{
	public class NegateInstruction : Instruction
	{
		public override bool Execute(State state) => false;

	   public override string ToString() => "!";
	}
}