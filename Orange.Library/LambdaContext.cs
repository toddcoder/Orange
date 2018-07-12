using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library
{
	public class LambdaContext
	{
		Stack<RegionBlock> deferredBlocks;

		public LambdaContext()
		{
			deferredBlocks = new Stack<RegionBlock>();
			Recurse = false;
		}

		public Arguments Arguments
		{
			get;
			set;
		}

		public ArrayYielder Yielder
		{
			get;
			set;
		}

		public void Defer(Block block)
		{
			var ns = new Region();
			RegionManager.Regions.Current.CopyAllVariablesTo(ns);
			deferredBlocks.Push(new RegionBlock(ns, block));
		}

		public void Undefer()
		{
			while (deferredBlocks.Count > 0)
			{
				var namespaceBlock = deferredBlocks.Pop();
				RegionManager.Regions.Push(namespaceBlock.Region, "undefer");
				namespaceBlock.Block.Evaluate();
				RegionManager.Regions.Pop("undefer");
			}
		}

		public bool Recurse
		{
			get;
			set;
		}
	}
}