using System.Linq;
using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Objects;

namespace Orange.Library.Verbs
{
	public class TailCall : Verb
	{
		const string LOCATION = "Tail call";
		const string VAR_ACCUMULATOR = "$accum";
		const string VAR_VALUE = "$value";

		Block block;
		Block modBlock;
		Block checkExpression;
		Block finalValue;
		Block modFinalValue;
		string variableName;

		public TailCall(Block block, Block checkExpression, Block finalValue)
		{
			this.block = block;
			this.checkExpression = checkExpression;
			this.finalValue = finalValue;
		}

		public void SetParameters(Parameters parameters)
		{
			variableName = parameters[0].Name;
		}

		static Block trimArgumentBlock(Block argumentBlock)
		{
			var newBlock = new Block();
			foreach (var verb in argumentBlock.TakeWhile(verb => !(verb is AppendToArray)))
				newBlock.Add(verb);
			return newBlock;
		}

		void modifyBlocks()
		{
/*			var builder = new CodeBuilder();
			var finalBuilder = new CodeBuilder();
			initialize(builder);
			initialize(finalBuilder);
			var count = block.Count;
			var foundArguments = false;
			foreach (var verb in block)
			{
				Invoke invoke;
				if (!verb.IsA(out invoke))
					continue;

				builder.Parenthesize(trimArgumentBlock(invoke.Arguments.ArgumentsBlock));
				finalBuilder.Parenthesize(finalValue);
				foundArguments = true;
				break;
			}
			Runtime.Assert(foundArguments, LOCATION, "Couldn't determine function call");
			builder.End();
			finalBuilder.End();
			var i = 0;
			while (i < count)
			{
				var verb = block[i];
				Push push;
				if (verb.IsA(out push))
				{
					var value = push.Value;
					Variable variable;
					if (value.IsA(out variable))
					{
						var name = variable.Name;
						if (name == variableName)
						{
							addAccumulator(builder);
							addAccumulator(finalBuilder);
							i++;
							continue;
						}
						if (i + 1 < count && block[i + 1] is Invoke)
						{
							addAccumulator(builder);
							addAccumulator(finalBuilder);
							i += 2;
							continue;
						}
					}
				}
				addVerb(builder, verb);
				addVerb(finalBuilder, verb);
				i++;
			}
			addComma(builder);
			addComma(finalBuilder);
			addValue(builder);
			addValue(finalBuilder);
			modBlock = builder.Block;
			modFinalValue = finalBuilder.Block;*/
			var builder = new CodeBuilder();
			builder.Define(VAR_ACCUMULATOR);
			builder.Assign();
			builder.Parenthesize(finalValue);
			builder.End();

			var actions = new CodeBuilder();
			actions.Variable(VAR_ACCUMULATOR);
			actions.Assign();

			var foundArguments = false;
			foreach (var verb in block)
			{
				Invoke invoke;
				if (!verb.IsA(out invoke))
					continue;

				builder.Parenthesize(trimArgumentBlock(invoke.Arguments.ArgumentsBlock));
				finalBuilder.Parenthesize(finalValue);
				foundArguments = true;
				break;
			}
			Runtime.Assert(foundArguments, LOCATION, "Couldn't determine function call");

		}

		static void addComma(CodeBuilder builder)
		{
			builder.Comma();
		}

		static void addVerb(CodeBuilder builder, Verb verb)
		{
			builder.Verb(verb);
		}

		static void addValue(CodeBuilder builder)
		{
			builder.Variable(VAR_VALUE);
		}

		static void addAccumulator(CodeBuilder builder)
		{
			builder.Variable(VAR_ACCUMULATOR);
		}

		static void initialize(CodeBuilder builder)
		{
			builder.Define(VAR_VALUE);
			builder.Assign();
		}

		public override Value Evaluate()
		{
			if (modBlock == null)
				modifyBlocks();
			var ns = Runtime.State.CurrentNamespace;
			block.AutoRegister = false;
			Runtime.State.RegisterBlock(modBlock);
			var accum = finalValue.Evaluate();//Runtime.State[variableName];
			var newValue = Runtime.State[variableName];
			for (var i = 0; i < Runtime.MAX_TAILCALL; i++)
			{
				ns.SetParameter(VAR_ACCUMULATOR, accum, _override: true);
				ns.SetParameter(variableName, newValue, _override: true);
				Value result;
				if (checkExpression.Evaluate().IsTrue)
				{
					ns.SetParameter(variableName, finalValue.Evaluate(), _override: true);
					result = modFinalValue.Evaluate();
					getValue(result, out accum, out newValue);
					Runtime.State.UnregisterBlock();
					return accum;
				}
				result = modBlock.Evaluate();
				getValue(result, out accum, out newValue);
			}
			Runtime.State.UnregisterBlock();
			Runtime.Throw(LOCATION, "Unbounded tail call");
			return null;
		}

		void getValue(Value result, out Value accum, out Value newValue)
		{
			Array array;
			Runtime.Assert(result.IsA(out array), LOCATION, "Internal error: result must be an array");
			accum = array[0];
			newValue = array[1];
		}

		public override ExpressionManager.VerbPresidenceType Presidence
		{
			get
			{
				return ExpressionManager.VerbPresidenceType.Assignment;
			}
		}
	}
}