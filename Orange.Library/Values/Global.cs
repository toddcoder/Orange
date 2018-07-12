using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Messages;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Values
{
	public class Global : Value, IMessageHandler
	{
		public override int Compare(Value value) => 0;

	   public override string Text { get; set; }

	   public override double Number { get; set; }

		public override ValueType Type => ValueType.Global;

	   public override bool IsTrue => false;

	   public override Value Clone() => new Global();

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
		{
			var stack = new Stack<Region>();
			Regions.ForEachRegion(stack.Push);
			while (stack.Count > 0)
			{
				var region = stack.Pop();
				if (!region.ContainsMessage(messageName))
					continue;
				handled = true;
				return region[messageName];
			}
			handled = false;
			return null;
		}

		public bool RespondsTo(string messageName)
		{
			var stack = new Stack<Region>();
			while (stack.Count > 0)
			{
				var region = stack.Pop();
				if (region.ContainsMessage(messageName))
					return true;
			}
			return false;
		}
	}
}