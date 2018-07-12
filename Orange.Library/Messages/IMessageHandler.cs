using Orange.Library.Values;

namespace Orange.Library.Messages
{
	public interface IMessageHandler
	{
		Value Send(Value value, string messageName, Arguments arguments, out bool handled);
		bool RespondsTo(string messageName);
	}
}