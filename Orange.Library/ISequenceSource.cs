using Orange.Library.Values;

namespace Orange.Library
{
	public interface ISequenceSource
	{
		Value Next();
		ISequenceSource Copy();
		Value Reset();
		int Limit
		{
			get;
		}
		Array Array
		{
			get;
		}
	}
}