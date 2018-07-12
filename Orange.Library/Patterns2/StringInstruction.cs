using System;

namespace Orange.Library.Patterns2
{
	public class StringInstruction : Instruction
	{
		protected string text;
		protected int textLength;
		protected int index;

		public StringInstruction(string text)
		{
			this.text = text;
			textLength = this.text.Length;
		}

		public override void Initialize()
		{
			index = 0;
		}

		public override bool Execute(State state)
		{
			var input = state.CurrentInput;
			if (index >= textLength)
			{
				state.InstructionIndex++;
				index = 0;
				return true;
			}
			var textChar = text[index];
			var inputChar = input[state.Position];
			if (!state.IgnoreCase)
			{
				textChar = char.ToUpper(textChar);
				inputChar = char.ToUpper(inputChar);
			}
			if (textChar == inputChar)
			{
				state.Position++;
				index++;
				return true;
			}
			state.InstructionIndex++;
			index = 0;
			return false;
		}

		public override bool ExecuteFirst(State state)
		{
			var comparison = state.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			index = state.CurrentInput.IndexOf(text, comparison);
			if (index > -1)
			{
				state.Position += textLength;
				state.InstructionIndex++;
				return true;
			}
			state.InstructionIndex++;
			return false;
		}

		public override string ToString() => "'" + text.Replace("'", "`'") + "'";
	}
}