using Orange.Library.Managers;
using Standard.Types.Objects;
using static Orange.Library.Arguments;

namespace Orange.Library.Values
{
	public class MessageData : Value
	{
		string message;

		public MessageData(string message, Value value)
		{
			this.message = message;
		   var executable = value.As<IExecutable>();
			if (executable.IsSome)
			{
				Executable = executable.Value;
				Cargo = null;
			}
			else
			{
				Executable = null;
				Cargo = value;
			}
		}

		public Value Cargo
		{
			get;
			set;
		}

		public IExecutable Executable
		{
			get;
			set;
		}

		public string Message => message;

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

		public override ValueType Type => ValueType.MessageData;

	   public override bool IsTrue => false;

	   public override Value Clone() => new MessageData(message, "")
	   {
	      Cargo = Cargo?.Clone(),
	      Executable = (IExecutable)((Value)Executable)?.Clone()
	   };

	   public Arguments GetArguments() => Executable != null ? FromExecutable(Executable, Cargo) : new Arguments(Cargo);

	   protected override void registerMessages(MessageManager manager)
		{
		}
	}
}