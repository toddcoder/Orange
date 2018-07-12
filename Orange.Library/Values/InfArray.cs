using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class InfArray : Array
	{
		const string LOCATION = "Infinite array";

		string variableName;
		Block block;

		public InfArray(string variableName, Block block)
		{
			this.variableName = variableName;
			this.block = block;
		}

		double getValue(int index)
		{
			RegionManager.Regions.SetLocal(variableName, index);
			var value = block.Evaluate();
			Runtime.RejectNull(value, LOCATION, "Block must return value");
			return value.Number;
		}

		public override Value this[int index]
		{
			get
			{
				RegionManager.Regions.Push("inf-array");
				for (var i = Length; i < index; i++)
				{
					var number = getValue(i);
					Add(number);
				}
				var result = getValue(index);
				RegionManager.Regions.Pop("inf-array");
				return result;
			}
			set => base[index] = value;
		}
	}
}