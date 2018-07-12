using System.Linq;
using System.Text;
using Orange.Library.Managers;
using Standard.Types.Arrays;
using Standard.Types.Enumerables;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class Buffer : Value
   {
      StringBuilder buffer;
      bool putting;

      public Buffer()
      {
         buffer = new StringBuilder();
         putting = false;
      }

      public Buffer(StringBuilder buffer, bool putting)
      {
         this.buffer = buffer;
         this.putting = putting;
      }

      public Buffer(string text)
      {
         buffer = new StringBuilder(text);
         putting = false;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => buffer.ToString();
         set { }
      }

      public override double Number
      {
         get => Text.ToDouble();
         set { }
      }

      public override ValueType Type => ValueType.Buffer;

      public override bool IsTrue => buffer.Length > 0;

      public override Value Clone() => new Buffer(buffer, putting);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "print", v => ((Buffer)v).Print());
         manager.RegisterMessage(this, "println", v => ((Buffer)v).Println());
         manager.RegisterMessage(this, "put", v => ((Buffer)v).Put());
         manager.RegisterMessage(this, "peek", v => ((Buffer)v).Peek());
         manager.RegisterMessage(this, "clear", v => ((Buffer)v).Clear());
         manager.RegisterProperty(this, "item", v => ((Buffer)v).GetItem(), v => ((Buffer)v).SetItem());
      }

      public override Value AlternateValue(string message) => buffer.ToString();

      public Value Print()
      {
         var values = Arguments.Values;
         switch (values.Length)
         {
            case 0:
               return NilValue;
            case 1:
               var asString = ValueAsString(values[0]);
               buffer.Append(asString);
               return asString;
            default:
               var text = values.Select(ValueAsString).Listify(State.FieldSeparator.Text);
               buffer.Append(text);
               return text;
         }
      }

      public Value Println()
      {
         putting = false;
         var values = Arguments.Values;
         switch (values.Length)
         {
            case 0:
               buffer.AppendLine();
               return NilValue;
            case 1:
               var asString = ValueAsString(values[0]);
               buffer.AppendLine(asString);
               return asString;
            default:
               var text = values.Select(ValueAsString).Listify(State.FieldSeparator.Text);
               buffer.AppendLine(text);
               return text;
         }
      }

      public Value Put()
      {
         var values = Arguments.Values;
         foreach (var value in values)
            Put(ValueAsString(value));

         return values.Listify(State.FieldSeparator.Text);
      }

      public Value Peek()
      {
         buffer.Append(Arguments[0]);
         return Arguments[0];
      }

      public void Print(string text)
      {
         if (buffer.Length != 0)
            buffer.Append(State.RecordSeparator.Text);
         buffer.Append(text);
         putting = false;
      }

      public void Put(string text)
      {
         if (putting)
            buffer.Append(State.FieldSeparator.Text);
         buffer.Append(text);
         putting = true;
      }

      public void Write(string text) => buffer.Append(text);

      public Value Clear()
      {
         buffer.Clear();
         return this;
      }

      public Value GetItem()
      {
         using (var popper = new RegionPopper(new Region(), "get-item"))
         {
            popper.Push();
            Regions.Current.SetParameter("$", buffer.Length);
            var arguments = new Array(Arguments.GetValues(buffer.Length));
            var iterator = new NSIteratorByLength(arguments.GetGenerator(), buffer.Length);
            var list = iterator.ToList();
            if (list.Count == 0)
               return "";

            return list.Select(getItem).Listify();
         }
      }

      public Value SetItem()
      {
         using (var popper = new RegionPopper(new Region(), "set-item"))
         {
            popper.Push();
            Regions.Current.SetParameter("$", buffer.Length);

            var popped = Arguments.Values.Pop();
            if (popped.IsNone)
               return this;

            var value = popped.Value.element.AssignmentValue();
            var values = popped.Value.array;

            var arguments = new Array(values);
            var iterator = new NSIteratorByLength(arguments.GetGenerator(), buffer.Length);
            var list = iterator.ToList();
            var text = value.Text;
            foreach (var index in list)
               setItem(index, text);
         }

         return this;
      }

      string getItem(Value index)
      {
         var i = index.Int;
         if (i.Between(0).Until(buffer.Length))
            return buffer[i].ToString();

         return "";
      }

      void setItem(Value index, string value)
      {
         var i = index.Int;
         var k = 0;
         for (var j = i; k < value.Length && j < buffer.Length; j++, k++)
            buffer[j] = value[k];
      }

      public override string ToString() => Text;
   }
}