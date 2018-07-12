using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Verbs
{
	public abstract class OperatorAssign : Verb
	{
		public override Value Evaluate()
		{
		   var stack = State.Stack;
		   var value = stack.Pop(true, Location);
			var variable = stack.Pop<Variable>(false, Location);
			var result = Exception(variable, value);
			if (result != null)
				return variable;
			result = evaluateByType(variable, value);
			result.AssignTo(variable);
			return variable;
		}

	   Value evaluateByType(Variable variable, Value value) => (variable.Value.IsArray || value.IsArray) && UseArrayVersion ? evaluateArray(variable, value) :
	      evaluate(variable, value);

	   public virtual Value Exception(Variable variable, Value value) => null;

	   Value evaluate(Variable variable, Value value)
		{
			switch (variable.Type)
			{
				case Value.ValueType.Big:
				case Value.ValueType.Object:
					return MessagingState.SendMessage(variable.Value, Message, new Arguments(value));
				default:
					return Execute(variable, value);
			}
		}

		Value evaluateArray(Variable variable, Value value)
		{
			Array yArray;
			var list = new List<Value>();

			var xValue = variable.Value;
			var variableName = VAR_ANONYMOUS + CompilerState.ObjectID();
			if (xValue.IsArray)
			{
				var xArray = (Array)xValue.SourceArray;
				Variable anonymousVariable;
				if (value.IsArray)
				{
					yArray = (Array)value.SourceArray;
					var minLength = Math.Min(xArray.Length, yArray.Length);
					for (var i = 0; i < minLength; i++)
					{
						var xItem = xArray[i];
						var yItem = yArray[i];
						Regions.Current.CreateVariable(variableName);
						anonymousVariable = new Variable(variableName)
						{
							Value = xItem
						};
						list.Add(evaluate(anonymousVariable, yItem));
					}
					Regions.Current.Remove(variableName);
					return new Array(list);
				}
				anonymousVariable = new Variable(variableName);
				foreach (var item in xArray)
				{
					anonymousVariable.Value = item.Value;
					list.Add(evaluate(anonymousVariable, value));
				}
				Regions.Current.Remove(variableName);
				return new Array(list);
			}
			if (value.IsArray)
			{
				yArray = (Array)value.SourceArray;
				list.AddRange(yArray.Select(i => evaluate(variable, i.Value)));
				return new Array(list);
			}
			return evaluate(variable, variable);
		}

		public abstract Value Execute(Variable variable, Value value);

		public override bool LeftToRight => false;

	   public abstract string Location
		{
			get;
		}

		public abstract string Message
		{
			get;
		}

		public virtual bool UseArrayVersion => true;
	}
}