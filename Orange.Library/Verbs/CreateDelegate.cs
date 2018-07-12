using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class CreateDelegate : Verb
	{
		string messageName;
		string target;
		string targetMessage;

		public CreateDelegate(string messageName, string target, string targetMessage)
		{
			this.messageName = messageName;
			this.target = target;
			this.targetMessage = targetMessage;
		}

		public override Value Evaluate()
		{
			RegionManager.Regions.CreateVariable(messageName);
			RegionManager.Regions[messageName] = new Delegate(target, targetMessage);
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Statement;
	}
}