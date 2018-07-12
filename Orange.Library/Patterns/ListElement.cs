using Orange.Library.Values;

namespace Orange.Library.Patterns
{
	public class ListElement : ArbNoElement
	{
		Pattern delimiterPattern;
		Pattern mainPattern;

		public ListElement(Pattern delimiterPattern, Pattern mainPattern)
			: base(new Pattern())
		{
			this.delimiterPattern = delimiterPattern;
			this.mainPattern = mainPattern;
			var delimiterElement = new PatternElement(this.delimiterPattern)
			{
				Alternate = new StringElement("")
			};
			var mainElement = new PatternElement(this.mainPattern);
			delimiterElement.Next = mainElement;
			pattern = new Pattern(delimiterElement);
		}

		public override Element Clone() => new ListElement(delimiterPattern, mainPattern);

	   public override string ToString() => $@"\{delimiterPattern} {mainPattern}";
	}
}