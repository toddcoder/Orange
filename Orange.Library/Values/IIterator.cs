namespace Orange.Library.Values
{
	public interface IIterator
	{
		Value Increment
		{
			get;
			set;
		}

		void Iterate(Lambda lambda);
	}
}