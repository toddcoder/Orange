namespace Orange.Library.Verbs
{
	public class NSExclusiveRange : NSRange
	{
		public NSExclusiveRange() => inclusive = false;

	   public override string ToString() => "...";
	}
}