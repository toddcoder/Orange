using System;
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Today : Verb
	{
		public override Value Evaluate() => DateTime.Today;

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Invoke;

	   public override string ToString() => "today";
	}
}