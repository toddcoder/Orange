using Orange.Library.Values;

namespace Orange.Library
{
	public interface IRepeatable
	{
		Value Repeat(int count);
	}
}