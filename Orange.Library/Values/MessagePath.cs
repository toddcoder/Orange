using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class MessagePath : Value
	{
		List<Message> messages;

		public MessagePath(Message message1, Message message2) => messages = new List<Message>
		{
		   message1,
		   message2
		};

	   public MessagePath(List<Message> messages) => this.messages = messages;

	   public MessagePath(Message message) => messages = new List<Message>
	   {
	      message
	   };

	   public MessagePath(string message) => messages = new List<Message>
	   {
	      new Message(message, new Arguments())
	   };

	   public MessagePath() => messages = new List<Message>();

	   public MessagePath(MessagePath path)
		{
			messages = new List<Message>();
			messages.AddRange(path.messages);
		}

		public override int Compare(Value value) => messages.Select(m => m.Compare(value)).Any(c => c == 0) ? 0 : 1;

	   public override string Text
		{
			get
			{
				return messages.Listify(" ~ ");
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

		public override ValueType Type => ValueType.MessagePath;

	   public override bool IsTrue => messages.Count > 0;

	   public override Value Clone() => new MessagePath(messages);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "concat", v => ((MessagePath)v).Concat());
			manager.RegisterMessage(this, "apply", v => ((MessagePath)v).Apply());
			manager.RegisterMessage(this, "invoke", v => ((MessagePath)v).Invoke());
		}

		public Value Concat()
		{
			var message = MessageFromArguments(Arguments);
			RejectNull(message, "Message chain", "Couldn't resolve message");
			messages.Add(message);
			return this;
		}

		public Value Apply() => Invoke(Arguments.ApplyValue);

	   public Value Invoke() => Invoke(Arguments[0]);

	   public Value Invoke(Value value)
		{
			foreach (var message in messages)
			{
				value = message.Invoke(value);
				if (value == null || value.IsNil)
					return new Nil();
			}
			return value;

		}

		public override string ToString() => messages.Select(m => m.MessageName).Listify(" ~ ");

	   public void Add(string message) => messages.Add(new Message(message, new Arguments()));

	   public void Add(Message message) => messages.Add(message);
	}
}