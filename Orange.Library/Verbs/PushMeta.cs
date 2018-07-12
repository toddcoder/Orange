using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class PushMeta : Verb
	{
		const string LOCATION = "Meta";

		public override Value Evaluate()
		{
			ValueStack stack = Runtime.State.Stack;
			Value reference = stack.Pop(true, LOCATION);
			Value target = stack.Pop(true, LOCATION);
			var obj = reference as Object;
			Array metadata;
			if (target.Meta == null)
			{
				metadata = new Array();
				target.Meta = metadata;
			}
			else
				metadata = target.Meta;
			if (obj != null)
			{
				string key = Runtime.State.IsBound(reference) ? Runtime.State.Unbind(reference) : reference.Type.ToString();
				metadata[key] = reference;
				return target;
			}
			return metadata[reference.Text];
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Apply;
			}
		}
	}
}