using Orange.Library.Managers;
using static Orange.Library.Managers.MessageManager;

namespace Orange.Library.Values
{
	public class Message : Value
	{
		string messageName;
		Arguments arguments;
		ValueType type;

		public Message(string messageName, Arguments arguments, ValueType type = ValueType.Message)
		{
			this.messageName = messageName;
			this.arguments = arguments;
			this.type = type;
		}

		public Message()
			: this("", new Arguments())
		{
		}

		public string MessageName => messageName;

	   public Arguments MessageArguments
		{
			get => arguments;
	      set => arguments = value;
	   }

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				return messageName;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return 0;
			}
			set
			{

			}
		}

		public Value SetType(ValueType type)
		{
			this.type = type;
			return this;
		}

		public override ValueType Type => type;

	   public override bool IsTrue => false;

	   public override Value Clone() => new Message(messageName, arguments);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "apply", v => ((Message)v).Apply());
			manager.RegisterMessage(this, "applyWhile", v => ((Message)v).Apply());
			manager.RegisterMessage(this, "name", v => ((Message)v).MessageName);
			manager.RegisterMessage(this, "applyNot", v => ((Message)v).ApplyNot());
			manager.RegisterMessage(this, "concat", v => ((Message)v).Concat());
		}

		public Value Apply() => Invoke(Arguments.ApplyValue);

	   public Value Invoke(Value recipient) => MessagingState.SendMessage(recipient, messageName, arguments);

	   public Value ApplyNot() => new MessageInvoke(Arguments.ApplyValue, this);

	   public Value Concat()
		{
			var otherMessage = Runtime.MessageFromArguments(Arguments);
			Runtime.RejectNull(otherMessage, "Message", "Couldn't resolve other message");
			return new MessagePath(this, otherMessage);
		}

		public override string ToString() => $"{messageName} {arguments}";
	}
}