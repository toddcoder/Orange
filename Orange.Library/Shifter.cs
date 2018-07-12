namespace Orange.Library
{
	public class Shifter
	{
		public static implicit operator Shifter(string source) => new Shifter(source);

	   string source;
		int index;

		public Shifter(string source)
		{
			this.source = source;
			index = 0;
		}

		public string Shift() => index < source.Length ? source[index++].ToString() : "";

	   public override string ToString() => index < source.Length ? source.Substring(index) : "";
	}
}