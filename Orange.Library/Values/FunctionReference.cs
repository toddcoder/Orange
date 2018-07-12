using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;

namespace Orange.Library.Values
{
	public class FunctionReference : Value, IWhere
	{
		string functionName;
		Lambda lambda;

		public FunctionReference(string functionName, Lambda lambda)
		{
			this.functionName = functionName;
			this.lambda = lambda;
		}

		public override int Compare(Value value) => 0;

	   public string FunctionName => functionName;

	   public Lambda Lambda => lambda;

	   public override string Text
		{
			get
			{
				return functionName;
			}
			set
			{
			}
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type => ValueType.FunctionReference;

	   public override bool IsTrue => false;

	   public override Value Clone() => new FunctionReference(functionName, lambda);

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public override string ToString()
		{
		   return $"{functionName}({lambda.Parameters.GetParameters().Select(p => p.Name).Listify()})";
		}

	   public Block Where
		{
			get => lambda.Where;
	      set => lambda.Where = value;
	   }
	}
}