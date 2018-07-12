namespace Orange.Library.Debugging
{
	public struct Position
	{
		public int StartIndex;
		public int Length;
		readonly int sum;

		public Position(int startIndex, int length)
		{
			StartIndex = startIndex;
			Length = length;
			sum = StartIndex + Length;
		}

		public override int GetHashCode() => sum.GetHashCode();

	   public override bool Equals(object obj)
		{
			if (obj is Position)
			{
				var other = (Position)obj;
				return StartIndex == other.StartIndex && Length == other.Length;
			}
			return false;
		}
	}
}