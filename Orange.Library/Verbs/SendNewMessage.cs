using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class SendNewMessage : Verb
	{
		string className;
		Block block;

		public SendNewMessage(string className, Block block)
		{
			this.className = className;
			this.block = block;
		}

		public override Value Evaluate()
		{
			var value = RegionManager.Regions[className];
			switch (value.Type)
			{
				case Value.ValueType.Class:
				{
					var argument = new Arguments(new Block(), block);
					return Runtime.SendMessage(value, "new", argument);
				}
			}
			return value;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Push;
	}
}