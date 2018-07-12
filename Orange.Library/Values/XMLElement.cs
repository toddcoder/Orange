using Orange.Library.Managers;
using System.Xml.Linq;
using System.Linq;

namespace Orange.Library.Values
{
   public class XMLElement : Value
   {
      XElement element;

      public XMLElement(XElement element) => this.element = element;

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => element.ToString();
         set { }
      }

      public override double Number
      {
         get => 0;
         set { }
      }

      public override ValueType Type => ValueType.XMLElement;

      public override bool IsTrue => false;

      public override Value Clone() => new XMLElement(element);

      public Value this[string name]
      {
         get
         {
            if (name == null)
               return "";

            return name.StartsWith("@") ? GetAttributeValue(name) : GetNodeValue(name);
         }
         set
         {
            if (name.StartsWith("@"))
               SetAttributeValue(name, value.Text);
            else
               SetNodeValue(name, value.Text);
         }
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "name", v => ((XMLElement)v).Name());
         manager.RegisterMessage(this, "nodes", v => ((XMLElement)v).Nodes());
         manager.RegisterMessage(this, "attr", v => ((XMLElement)v).Attributes());
         manager.RegisterMessage(this, "value", v => ((XMLElement)v).InnerText());
      }

      public Value Name() => element.Name.LocalName;

      public Value Nodes() => new Array(element.Elements().Select(e => new XMLElement(e)).ToArray());

      public Value Attributes() => new Array(element.Attributes().Select(a => new KeyedValue(a.Name.LocalName, a.Value)).ToArray());

      public Value InnerText() => element.Value;

      public bool ContainsNode(string nodeName) => element.Elements(nodeName).Any();

      public string GetNodeValue(string nodeName)
      {
         var result = element.Elements(nodeName).FirstOrDefault();
         return result?.Value ?? "";
      }

      public void SetNodeValue(string nodeName, string value)
      {
         var result = element.Elements(nodeName).FirstOrDefault();
         if (result != null)
            result.Value = value;
      }

      public string GetAttributeValue(string attributeName)
      {
         if (attributeName.StartsWith("@"))
            attributeName = attributeName.Substring(1);
         var result = element.Attribute(attributeName);
         return result?.Value ?? "";
      }

      public void SetAttributeValue(string attributeName, string value)
      {
         if (attributeName.StartsWith("@"))
            attributeName = attributeName.Substring(1);
         var result = element.Attribute(attributeName);
         if (result != null)
            result.Value = value;
      }

      public override string ToString() => element.ToString();
   }
}