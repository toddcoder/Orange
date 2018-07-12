using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Internet.XML;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Values
{
   public class XML : Value
   {
      Value name;
      Block attributes;
      Value innerText;
      List<XML> children;
      Queue<Region> namespaces;

      public XML(Value name, Block attributes, Value innerText)
      {
         this.name = name;
         this.attributes = attributes;
         this.innerText = innerText;
         children = new List<XML>();
         namespaces = new Queue<Region>();
      }

      public XML()
      {
         name = "$unknown";
         attributes = new Block();
         innerText = "";
      }

      public override int Compare(Value value) => 0;

      public void AddChild(XML child) => children.Add(child);

      public Block Children { get; set; }

      public Array ChildrenArray { get; set; }

      public virtual Element Element
      {
         get
         {
            var hasContent = namespaces.Count > 0;
            if (hasContent)
            {
               var region = namespaces.Dequeue();
               Regions.Push(region, "xml");
            }
            var element = new Element { Name = name.IsVariable ? ((Variable)name).Name : name.Text };
            if (attributes != null)
            {
               attributes.ResolveVariables = true;
               var attributesValue = attributes.Evaluate();
               if (attributesValue != null)
                  switch (attributesValue.Type)
                  {
                     case ValueType.Array:
                        var array = (Array)attributesValue;
                        foreach (var item in array)
                           element.Attributes.Add(item.Key, item.Value.Text);

                        break;
                     case ValueType.KeyedValue:
                        var keyedValue = (KeyedValue)attributesValue;
                        element.Attributes.Add(keyedValue.Key, keyedValue.Value.Text);
                        break;
                  }
            }

            if (innerText != null)
               element.Text = innerText.Text;
            var childrenValue = Children?.Evaluate();
            if (childrenValue != null)
            {
               if (childrenValue is XML xml)
                  children.Add(xml);
               if (childrenValue is Array array)
                  addArray(array);
            }
            if (ChildrenArray != null)
               addArray(ChildrenArray);
            foreach (var child in children)
               element.Children.Add(child.Element);

            if (hasContent)
               Regions.Pop("element");
            return element;
         }
      }

      void addArray(Array array)
      {
         foreach (var xml in array
            .Select(item => new { item, value = item.Value })
            .Where(t => t.item.Value.Type == ValueType.XML)
            .Select(t => (XML)t.value))
            children.Add(xml);
      }

      public override string Text
      {
         get => Element.ToString();
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.XML;

      public override bool IsTrue => false;

      public override Value Resolve() => this;

      public override Value AssignmentValue() => this;

      public override Value Clone() => new XML(name.Clone(), (Block)attributes?.Clone(), innerText?.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "tidy", v => ((XML)v).Tidy());
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((XML)v).Apply());
         manager.RegisterMessage(this, "add-child", v => ((XML)v).AddChild());
      }

      public Value AddChild()
      {
         var xml = (XML)Arguments[0];
         children.Add(xml);
         return this;
      }

      public Value Apply()
      {
         var region = new Region();
         var parameters = (Parameters)Arguments.ApplyValue;
         foreach (var variableName in parameters.VariableNames)
            region.SetLocal(variableName, Regions[variableName].AssignmentValue());

         namespaces.Enqueue(region);
         return this;
      }

      public Value Tidy() => Text.Tidy(false);

      public override string ToString() => Element.ToString();
   }
}