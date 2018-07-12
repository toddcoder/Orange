using Standard.Computer;
using Standard.Types.Collections;

namespace Orange.Library
{
	public class InteractiveFileCache : IFileCache
	{
		Hash<string, string> cache;

		public InteractiveFileCache() => cache = new Hash<string, string>();

	   public string GetFile(string fileName) => cache.Find(fileName, f => ((FileName)f).Text);

	   public void Clear() => cache.Clear();
	}
}