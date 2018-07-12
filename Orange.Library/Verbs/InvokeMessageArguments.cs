using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class InvokeMessageArguments : Verb
	{
		List<string> messages;
		List<Block> values;

		public InvokeMessageArguments()
		{
			messages = new List<string>();
			values = new List<Block>();
		}

		public void AddMessage(string message)
		{
			messages.Add(message);
		}

		public void AddValue(Block block)
		{
			block.AutoRegister = false;
			values.Add(block);
		}

		public override Value Evaluate()
		{
			var message = messages.Select(m => m + "_").Listify("");
			var arguments = new Arguments(values.Select(v => v.Evaluate()).ToArray());
			var target = State.Stack.Pop(true, "Invoke message arguments");
			return SendMessage(target, message, arguments);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Push;
	}
}