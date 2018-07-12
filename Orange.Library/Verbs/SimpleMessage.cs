using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class SimpleMessage : Verb
	{
		string message;
		bool popForValue;
	   bool swap;
	   bool self;

		public SimpleMessage(string message, bool popForValue, bool self, bool swap = false)
		{
			this.message = message;
			this.popForValue = popForValue;
		   this.self = self;
		   this.swap = swap;
		}

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var right = stack.Pop(true, message);
		   if (self)
		      right = right.Self;
			var left = stack.Pop(true, message);
		   if (self)
		      left = left.Self;
		   if (swap)
		   {
		      var temp = left;
		      left = right;
		      right = temp;
		   }
			var arguments = FromValue(right, false);
			if (popForValue)
			{
				var value = stack.Pop(true, message);
			   if (self)
			      value = value.Self;
				arguments.AddArgument(value);
			}
			return SendMessage(left, message, arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => message;
	}
}