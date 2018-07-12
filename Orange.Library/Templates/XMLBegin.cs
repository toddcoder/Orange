using System.Collections.Generic;
using System.IO;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Orange.Library.Templates
{
	public class XMLBegin : Item, IXMLStack
	{
		string attributes;

		public XMLBegin(string text, string attributes)
			: base(text)
		{
			this.attributes = attributes;
		}

		public XMLBegin()
			: this("", "")
		{
		}

		public Stack<string> Stack
		{
			get;
			set;
		}

		public override void Render(StringWriter writer, string variableName)
		{
			Stack.Push(text);
			writer.Write($"|<{text}");
			if (attributes.IsNotEmpty())
			{
				var index = 0;
				var block = OrangeCompiler.ParseBlock(attributes, ref index, "");
				var value = block.Evaluate();
				if (value != null)
					switch (value.Type)
					{
						case Value.ValueType.Array:
							var attributesText = getAttributes((Array)value.Resolve());
							writer.WriteLine($" {attributesText}>|.write('{variableName}');");
							break;
						case Value.ValueType.KeyedValue:
							var attributeText = getAttribute((KeyedValue)value);
							writer.WriteLine($" {attributeText}>|.write('{variableName}');");
							break;
						default:
							writer.WriteLine($">|.write('{variableName}');");
							break;
					}
				else
					writer.WriteLine($">|.write('{variableName}');");
			}
			else
				writer.WriteLine($">|.write('{variableName}');");
		}

		static string getAttributes(Array array) => array.Select(i => $"{i.Key}=\"{i.Value.Text}\"").Listify(" ");

	   static string getAttribute(KeyedValue keyedValue) => $"{keyedValue.Key}=\"{keyedValue.Text}\"";
	}
}