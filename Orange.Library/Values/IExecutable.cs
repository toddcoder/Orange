namespace Orange.Library.Values
{
	public interface IExecutable
	{
		Value Evaluate();
		Block Action
		{
			get;
		}

		Lambda AsLambda
		{
			get;
		}

	   Parameters Parameters
	   {
	      get;
	   }
	}
}