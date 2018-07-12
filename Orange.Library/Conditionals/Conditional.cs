using Orange.Library.Patterns;
using Orange.Library.Values;
using Standard.Types.Strings;

namespace Orange.Library.Conditionals
{
	public class Conditional
	{
		Lambda lambda;

		public Conditional(Lambda lambda) => this.lambda = lambda;

	   public virtual bool Evaluate(Element element)
		{
			Runtime.State.SaveWorkingInput();
			var text = Runtime.State.Input.Skip(element.Index).Take(element.Length);
			var value = lambda.Evaluate(new Arguments(text));
			if (value == null)
				return true;
			var result = value.IsTrue;
			Runtime.State.RestoreWorkingInput();
			return result;
		}
	}
}