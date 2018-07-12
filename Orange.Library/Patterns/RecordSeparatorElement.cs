namespace Orange.Library.Patterns
{
	public class RecordSeparatorElement : StringElement
	{
		public RecordSeparatorElement()
			: base("")
		{
		}

		public override bool Evaluate(string input)
		{
			text = Runtime.State.RecordSeparator.Text;
			return base.Evaluate(input);
		}
	}
}