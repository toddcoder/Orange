using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class ParserResult
	{
		public Verb Verb
		{
			get;
			set;
		}

		public int Position
		{
			get;
			set;
		}

		public Value Value
		{
			get;
			set;
		}

		public object Cargo
		{
			get;
			set;
		}

		public Verb Next
		{
			get;
			set;
		}

		public List<Verb> Verbs
		{
			get;
			set;
		}

		public override string ToString() => $"{Verb}@{Position}";
	}
}