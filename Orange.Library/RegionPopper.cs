using System;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library
{
	public class RegionPopper : IDisposable
	{
		protected Region region;
		protected string name;
		protected bool popped;

		public RegionPopper(Region region, string name)
		{
			this.region = region;
			this.name = name;
			popped = false;
		}

		public virtual void Push() => Regions.Push(region, name);

	   public void Dispose()
		{
			if (!popped)
				Pop();
		}

		public virtual void Pop()
		{
			Regions.Pop(name);
			popped = true;
		}
	}
}