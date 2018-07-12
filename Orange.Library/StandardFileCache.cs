using Standard.Computer;

namespace Orange.Library
{
	public class StandardFileCache : IFileCache
	{
		public string GetFile(string fileName)
		{
			FileName file = fileName;
			return file.Text;
		}

		public void Clear()
		{
		}
	}
}