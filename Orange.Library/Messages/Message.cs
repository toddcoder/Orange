using Orange.Library.Values;

namespace Orange.Library.Messages
{
	public abstract class Message
	{
		public string Name
		{
			get;
			set;
		}

		public Arguments Arguments
		{
			get;
			set;
		}

		public abstract Value Evaluate(Value value);

		public bool ResolveArguments
		{
			get;
			set;
		}
	}
}