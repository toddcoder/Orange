using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;

namespace Orange.Library.Values
{
	public class MessageArguments : Value
	{
		List<string> messages;
		List<Value> values;

		public MessageArguments()
		{
			messages = new List<string>();
			values = new List<Value>();
		}

	   public MessageArguments(List<string> messages, List<Value> values)
	   {
	      this.messages = messages;
	      this.values = values;
	   }

		public void Add(string message, Value value)
		{
			messages.Add(message);
			values.Add(value);
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.MessageArguments;

	   public override bool IsTrue => false;

	   public override Value Clone() => new MessageArguments(messages, values);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "apply", v => ((MessageArguments)v).Apply());
			manager.RegisterMessage(this, "applyIf", v => ((MessageArguments)v).ApplyIf());
			manager.RegisterMessage(this, "invoke", v => ((MessageArguments)v).Invoke());
		}

		Message getMessage()
		{
			var messageName = messages.Select(m => m + "_").Listify("");
			var arguments = new Arguments(values.ToArray());
			return new Message(messageName, arguments);
		}

		public Value Apply()
		{
			var value = Arguments.ApplyValue;
			var message = getMessage();
			return Runtime.SendMessage(value, message);
		}

		public Value ApplyIf()
		{
			var value = Arguments.ApplyValue;
			var message = getMessage();
			bool responds;
			var result = MessageManager.MessagingState.SendMessageIf(value, message.MessageName, message.MessageArguments, out responds);
			return responds ? result : new Nil();
		}

		public Value Invoke()
		{
			var message = getMessage();
			var value = RegionManager.Regions[message.MessageName];
			return Runtime.SendMessage(value, "invoke", message.MessageArguments);
		}

		public override string ToString() => messages.Select((m, i) => $"{m}:{values[i]}").Listify(" ");

	   public Value SendMessage(Value value)
		{
			var message = getMessage();
			return Runtime.SendMessage(value, message);
		}
	}
}