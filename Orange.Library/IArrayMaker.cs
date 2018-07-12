using Orange.Library.Values;

namespace Orange.Library
{
	public interface IArrayMaker
	{
		Value Seed
		{
			get;
			set;
		}

		bool More
		{
			get;
			set;
		}

		Value Next();
	}
}