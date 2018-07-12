using System;
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Now : Verb
	{
		public override Value Evaluate() => DateTime.Now;

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Invoke;

	   public override string ToString() => "now";
	}
}