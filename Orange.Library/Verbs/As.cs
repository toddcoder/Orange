using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class As : Verb
	{
		const string LOCATION = "As";

		public override Value Evaluate()
		{
			var stack = State.Stack;
			var y = stack.Pop(true, LOCATION);
			var x = stack.Pop(true, LOCATION);
         if (x is Object obj)
         {
            if (y is Class cls && obj.Class.IsChildOf(cls))
               return new Some(obj);
            if (y is Trait trait && (obj.Class.ImplementsTrait(trait) || obj.ImplementsInterface(trait)))
               return new Some(obj);
         }

		   return new None();
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "as";
	}
}