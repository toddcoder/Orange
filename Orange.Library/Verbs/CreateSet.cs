using System.Linq;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class CreateSet : Verb
	{
		Block block;

		public CreateSet(Block block) => this.block = block;

	   public override Value Evaluate()
		{
			var result = block.Evaluate();
		   var array = GeneratorToArray(result);
			array = (Array)array.Flatten();
			return new Set(array);
		}

		public Set Create()
		{
			var array = new Array();
		   foreach (var verb in block.AsAdded.Where(verb => !(verb is AppendToArray)))
		      if (verb is Push push)
		         array.Add(push.Value);

		   return new Set(array);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Push;
	}
}