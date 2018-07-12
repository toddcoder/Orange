using Orange.Library.Managers;
using Standard.Internet.XML;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class IndirectXML : XML
	{
		const string LOCATION = "Indirect XML";

		string variableName;

		public IndirectXML(string variableName)
			: base(null, null, null) => this.variableName = variableName;

	   public IndirectXML()
			: this("$unknown")
		{
		}

		public override Element Element
		{
			get
			{
				var value = RegionManager.Regions[variableName];
				Assert(value.Type == ValueType.XML, LOCATION, $"Variable {variableName} doesn't result in XML");
				return ((XML)value).Element;
			}
		}
	}
}