namespace Orange.Library.Patterns2
{
   public class LengthInstruction : Instruction
   {
      protected int count;
      protected int index;

      public LengthInstruction(int count)
      {
         this.count = count;
         index = 0;
      }

      public override void Initialize() => index = 0;

      public override bool Execute(State state)
      {
         if (index <= count)
         {
            state.Position++;
            return true;
         }

         state.InstructionIndex++;
         return true;
      }

      public override string ToString() => count == 1 ? "." : count + ".";
   }
}