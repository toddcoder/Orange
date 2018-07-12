using Orange.Library.Values;

namespace Orange.Library
{
	public interface IGenerator
	{
		void Before();

		Value Next(int index);

		void End();
	}
}