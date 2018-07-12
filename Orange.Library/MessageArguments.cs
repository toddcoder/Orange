using System;
using System.Collections.Generic;
using Orange.Library.Values;
using Array = Orange.Library.Values.Array;

namespace Orange.Library
{
	public class MessageArguments
	{
		List<string> variableNames;

		public MessageArguments(IEnumerable<string> variableNames)
		{
			this.variableNames = new List<string>();
			this.variableNames.AddRange(variableNames);
		}

		public void Begin()
		{
			Runtime.State.PushNamespace("message-begin");
		}

		public void SetValues(Block actualArguments, Block executable, bool resolveArguments)
		{
			Runtime.State.SetLocal(Runtime.VAR_ARGUMENTS, actualArguments);
			Array values = resolveArguments ? actualArguments.ToActualArguments() : actualArguments.ToDefinedArguments();
			int min = Math.Min(variableNames.Count, values.Length);
			for (var i = 0; i < min; i++)
			{
				Value value = values[i];
				Runtime.State.SetLocal(variableNames[i], value.AssignmentValue());
			}
			Runtime.State.SetLocal(Runtime.VAR_BLOCK, executable ?? new NullBlock());

			Array variables = values;
			if (resolveArguments)
				variables = actualArguments.ToDefinedArguments();

			for (var i = 0; i < min; i++)
			{
				Value value = variables[i];
				Runtime.State.SetLocal(Runtime.VAR_INDIRECT + i, value.IsVariable ? ((Variable)value).Name : "");
			}
		}

		public void End()
		{
			Runtime.State.PopNamespace("message-end");
		}
	}
}