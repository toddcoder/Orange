using static Orange.Library.Compiler;

namespace Orange.Library.Patterns
{
	public class FailElement : Element
	{
		public override bool Evaluate(string input) => false;

	   public override bool Failed => true;

	   public override Element Clone() => new FailElement
		{
		   Next = cloneNext(),
		   Alternate = cloneAlternate(),
		   Replacement = cloneReplacement(),
		   ID = CompilerState.ObjectID()
		};

	   public override string ToString() => ";";
	}
}