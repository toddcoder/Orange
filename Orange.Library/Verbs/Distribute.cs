using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Orange.Library.Verbs
{
	public class Distribute : Verb
	{
		const string STR_LOCATION = "Distribute";

		public override Value Evaluate()
		{
			string[] fields = Runtime.State.FieldPattern.Split(Runtime.State.Stack.Pop(true, STR_LOCATION).Text);
			var fieldQueue = new Queue<string>(fields);
			var parameters = (Parameters)Runtime.State.Stack.Pop(true, STR_LOCATION);
			string[] variableNames = parameters.VariableNames;
			var varQueue = new Queue<string>(variableNames);
			while (varQueue.Count > 0)
			{
				string variableName = varQueue.Dequeue();
				string field = fieldQueue.Count > 0 ? fieldQueue.Dequeue() : "";
				if (varQueue.Count == 0 && fieldQueue.Count > 0)
				{
					var fieldList = new List<string>
					{
						field
					};
					while (fieldQueue.Count > 0)
						fieldList.Add(fieldQueue.Dequeue());
					field = fieldList.Listify(Runtime.State.FieldSeparator.Text);
				}
				RegionManager.Regions[variableName] = field;
			}

			return new Array(fields);
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Assignment;
			}
		}

		public override bool LeftToRight
		{
			get
			{
				return false;
			}
		}

		public override string ToString()
		{
			return ":=";
		}
	}
}