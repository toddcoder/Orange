namespace Orange.Library.Values
{
	public class XMLElementIndexer : Variable
	{
		XMLElement element;
		string nodeName;

		public XMLElementIndexer(XMLElement element, string nodeName)
			: base("")
		{
			this.element = element;
			this.nodeName = nodeName;
		}

		public override Value Value
		{
			get => element[nodeName];
		   set => element[nodeName] = value;
		}

		public override string ContainerType => ValueType.XMLElement.ToString();

	   public override Value MessageTarget(string message) => this;
	}
}