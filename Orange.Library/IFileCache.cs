namespace Orange.Library
{
	public interface IFileCache
	{
		string GetFile(string fileName);
		void Clear();
	}
}