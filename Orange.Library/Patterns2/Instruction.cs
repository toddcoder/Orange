using Orange.Library.Conditionals;
using Orange.Library.Replacements;

namespace Orange.Library.Patterns2
{
	public abstract class Instruction
	{
		public virtual Instruction Alternate
		{
			get;
			set;
		}

		public bool Not
		{
			get;
			set;
		}

		public long ID
		{
			get;
			set;
		}

		public IReplacement Replacement
		{
			get;
			set;
		}

		public abstract bool Execute(State state);

		public virtual bool ExecuteFirst(State state) => Execute(state);

	   public virtual void Initialize()
		{
		}

		public Conditional Conditional
		{
			get;
			set;
		}

		public void AppendAlternate(Instruction instruction)
		{
			if (Alternate == null)
				Alternate = instruction;
			else
				Alternate.AppendAlternate(instruction);
		}
	}
}