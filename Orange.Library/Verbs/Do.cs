using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Do : Verb
	{
		public override Value Evaluate()
		{
			var value = Runtime.State.Stack.Pop(true, "Do");
			return new MessageData("do", value);
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Apply;
			}
		}

		public override bool LeftToRight
		{
			get
			{
				return false;
			}
		}

		public override string ToString()
		{
			return "do";
		}
	}
}