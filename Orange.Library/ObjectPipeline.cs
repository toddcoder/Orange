using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library
{
	public class ObjectPipeline : IPipelineSource
	{
		Object obj;

		public ObjectPipeline(Object obj)
		{
			this.obj = obj;
		}

		public Value Next()
		{
			return MessageManager.State.SendMessage(obj, "next", new Arguments());
		}

		public IPipelineSource Copy()
		{
			return new ObjectPipeline((Object)obj.Clone());
		}

		public Value Reset()
		{
			return MessageManager.State.SendMessage(obj, "reset", new Arguments());
		}

		public int Limit
		{
			get
			{
				return (int)MessageManager.State.SendMessage(obj, "limit", new Arguments()).Number;
			}
		}
	}
}