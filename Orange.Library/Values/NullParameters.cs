namespace Orange.Library.Values
{
	public class NullParameters : Parameters
	{
		public NullParameters()
			: base(new Parameter[0])
		{
		}

		public override bool Usable => false;
	}
}