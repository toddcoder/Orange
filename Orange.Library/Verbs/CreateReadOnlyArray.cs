using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class CreateReadOnlyArray : Verb
	{
		Block block;

		public CreateReadOnlyArray(Block block) => this.block = block;

	   public CreateReadOnlyArray() => block = new Block();

	   public override Value Evaluate()
		{
			var value = block.Evaluate();
			var array = value.IsArray ? (Array)value.SourceArray : newArray(value);
			return new ReadOnlyArray(array);
		}

		static Array newArray(Value value)
		{
			if (value.IsEmpty)
				return new Array();
			return new Array
			{
				value
			};
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => $"^({block})";
	}
}