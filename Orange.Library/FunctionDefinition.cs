using Orange.Library.Values;

namespace Orange.Library
{
	public class FunctionDefinition
	{
		public static FunctionDefinition DefaultConstructor(string name)
		{
			return new FunctionDefinition(name, new NullParameters(), new Block());
		}

		protected string name;
		protected Parameters Parameters;
		protected Block block;

		public FunctionDefinition(string name, Parameters parameters, Block block,
			Class.VisibilityType visibility = Class.VisibilityType.Public)
		{
			this.name = name;
			Parameters = parameters;
			this.block = block;
			Visibility = visibility;
			AutoLocal = true;
		}

		public virtual Value Execute(Arguments arguments)
		{
			setArguments(arguments);
			Value result = block.Evaluate();
			result = Runtime.State.UseReturnValue(result);
			if (AutoLocal)
				Runtime.State.PopNamespace("func-exec");
			return result;
		}

		protected virtual void setArguments(Arguments arguments)
		{
			if (AutoLocal)
				Runtime.State.PushNamespace("func-exec");
			Parameters.SetArguments(arguments);
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public bool AutoLocal
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("def {0}({1}){2}", name, Parameters, block);
		}

		public Class.VisibilityType Visibility
		{
			get;
			set;
		}
	}
}