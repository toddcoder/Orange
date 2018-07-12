using Orange.Library.Values;

namespace Orange.Library
{
	public interface IGetInvokeableReference
	{
		InvokeableReference InvokeableReference(string message, bool isObject);
	}
}