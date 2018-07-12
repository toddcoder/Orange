using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class SetOption : Verb
	{
		const string LOCATION = "Set option";

		static Value setOption(Value value, Value.OptionType option)
		{
			value.SetOption(option);
			return value;
		}

		public override Value Evaluate()
		{
		   var stack = State.Stack;
		   var option = stack.Pop<Variable>(false, LOCATION);
			var value = stack.Pop(true, LOCATION);
			var name = option.Name.ToLower();
			switch (name)
			{
				case "rjust":
					return setOption(value, Value.OptionType.RJust);
				case "ljust":
					return setOption(value, Value.OptionType.LJust);
				case "center":
					return setOption(value, Value.OptionType.Center);
				case "max":
					return setOption(value, Value.OptionType.Max);
				case "no-pad":
					return setOption(value, Value.OptionType.NoPad);
				case "case":
					return setOption(value, Value.OptionType.Case);
				case "anchor":
					return setOption(value, Value.OptionType.Anchor);
				case "num":
					return setOption(value, Value.OptionType.Numeric);
				case "desc":
					return setOption(value, Value.OptionType.Descending);
				case "flat":
					return setOption(value, Value.OptionType.Flat);
				default:
					Throw(LOCATION, $"Didn't understand option {option.Name}");
					return null;
			}
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => "!-";
	}
}