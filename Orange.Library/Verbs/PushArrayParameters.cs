using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class PushArrayParameters : Verb
	{
		const string LOCATION = "Push array comprehension";

		Parameters parameters;
		Block block;
		List<string> unpackedVariables;

		public PushArrayParameters(Parameters parameters, Block block, List<string> unpackedVariables)
		{
			this.parameters = parameters;
			this.block = block;
			this.unpackedVariables = unpackedVariables;
		}

		public PushArrayParameters()
			: this(new Parameters(), new Block(), new List<string>())
		{
		}

		public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, LOCATION);
			switch (value.Type)
			{
			   case ValueType.Block:
			      return new Comprehension((Block)value, parameters);
			   case ValueType.Comprehension:
			      value = value.SourceArray;
			      break;
			}

		   if (value.IsArray && parameters.Splatting)
				Assign.FromFieldsLocal((Array)value.SourceArray, parameters);
			else
			{
				var valueVariable = parameters.VariableName(0);
				var keyVariable = parameters.VariableName(1);
				var indexVariable = parameters.VariableName(2);

				var names = State.PushDefaultParameterNames();
				if (valueVariable.IsNotEmpty())
					names.ValueVariable = valueVariable;
				if (keyVariable.IsNotEmpty())
					names.KeyVariable = keyVariable;
				if (indexVariable.IsNotEmpty())
					names.IndexVariable = indexVariable;
				if (unpackedVariables.Count > 0)
					names.UnpackedVariables = unpackedVariables;
				names.IsUpperLevel = true;
			}
		   if (block == null)
		      return value;

		   try
			{
				block.AutoRegister = false;
				State.RegisterBlock(block);
				State.Stack.Push(value);
				var evaluated = block.Evaluate();
				State.UnregisterBlock();
				return evaluated;
			}
			finally
			{
				State.PopDefaultParameterNames();
			}
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => $"-> |{parameters}|";
	}
}