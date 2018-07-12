namespace Orange.Library.Patterns
{
	public class AlternateData
	{
		 public Element Alternate
		 {
			 get;
			 set;
		 }

		 public int Position
		 {
			 get;
			 set;
		 }

		 public Element Next
		 {
			 get;
			 set;
		 }

		 public Element OwnerNext
		 {
			 get;
			 set;
		 }

		 public long ElementID
		 {
			 get;
			 set;
		 }

		public bool Exit
		{
			get;
			set;
		}

		public override string ToString() => $"{Alternate} -> {Next} @ {Position}";
	}
}