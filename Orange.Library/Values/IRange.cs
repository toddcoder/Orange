namespace Orange.Library.Values
{
	public interface IRange
	{
		Value	Increment
		{
			get;
			set;
		}

		void SetStart(Value start);
		void SetStop(Value stop);

		Value Start
		{
			get;
		}

		Value Stop
		{
			get;
		}
	}
}