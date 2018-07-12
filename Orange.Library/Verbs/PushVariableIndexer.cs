using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class PushVariableIndexer : Verb
	{
		const string LOCATION = "Push varible indexer";

		public override Value Evaluate()
		{
			ValueStack stack = Runtime.State.Stack;
			Value index = stack.Pop(false, LOCATION);
			Value value = stack.Pop(true, LOCATION);
			var variable = index as Variable;
			string name = variable != null ? variable.Value.Text : value.Text;
			Block block = Block.GuaranteeBlock(name);
			return value.IsArray ? new KeyIndexer((Array)value.SourceArray, block) : value;
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Apply;
			}
		}

		public override string ToString()
		{
			return ".$";
		}
	}
}