using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Types.Collections;

namespace Orange.Library.Managers
{
	public class LoopManager
	{
		Hash<long, int> indexes;
		Stack<long> idStack;

		public LoopManager()
		{
		   indexes = new Hash<long, int>();
			idStack = new Stack<long>();
		}

		public long CurrentID => idStack.Peek();

	   public int CurrentIndex
		{
			get
			{
			   return CurrentID > -1 ? indexes.Find(CurrentID, l => -1) : -1;
			}
			set
			{
				if (CurrentID > -1)
					indexes[CurrentID] = value;
			}
		}

		public void Register(Block block)
		{
			var id = block.ID;
			if (!indexes.ContainsKey(id))
				indexes[id] = -1;
			idStack.Push(id);
		}

		public void Register() => idStack.Push(-1);

	   public void Unregister() => idStack.Pop();
	}
}