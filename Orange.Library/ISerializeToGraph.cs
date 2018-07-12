using Standard.Configurations;
using Standard.ObjectGraphs;

namespace Orange.Library
{
	public interface ISerializeToGraph
	{
		ObjectGraph ToGraph(string name);

		void FromGraph(ObjectGraph graph);
	}
}