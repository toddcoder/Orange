using System;
using System.Linq;
using Orange.Library.Managers;
using System.Text;
using Standard.Types.Arrays;
using Standard.Types.Enumerables;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Values
{
   public class StringBuffer : Value
   {
      StringBuilder builder;
      bool putting;

      public StringBuffer(string starter)
      {
         builder = new StringBuilder(starter);
         putting = false;
      }

      public override int Compare(Value value) => string.Compare(Text, value.Text, StringComparison.Ordinal);

      public override string Text
      {
         get
         {
            return builder.ToString();
         }
         set
         {
         }
      }

      public override double Number
      {
         get
         {
            return Text.ToDouble();
         }
         set
         {
         }
      }

      public override ValueType Type => ValueType.StringBuffer;

      public override bool IsTrue => Text.IsNotEmpty();

      public override Value Clone() => new StringBuffer(Text);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "clear", v => ((StringBuffer)v).Clear());
         manager.RegisterMessage(this, "print", v => ((StringBuffer)v).Print());
         manager.RegisterMessage(this, "put", v => ((StringBuffer)v).Put());
         manager.RegisterMessage(this, "write", v => ((StringBuffer)v).Write());
         manager.RegisterMessage(this, "append", v => ((StringBuffer)v).Write());
         manager.RegisterProperty(this, "item", v => ((StringBuffer)v).GetItem(), v => ((StringBuffer)v).SetItem());
      }

      public Value Clear()
      {
         builder.Clear();
         return this;
      }

      public Value Print()
      {
         if (builder.Length != 0)
            builder.Append(Runtime.State.RecordSeparator.Text);
         builder.Append(Arguments[0].Text);
         putting = false;
         return this;
      }

      public Value Put()
      {
         if (putting)
            builder.Append(Runtime.State.FieldSeparator.Text);
         builder.Append(Arguments[0].Text);
         putting = true;
         return this;
      }

      public Value Write()
      {
         builder.Append(Arguments[0].Text);
         return this;
      }

      public Value Append(Value value)
      {
         builder.Append(Runtime.Text(value));
         return this;
      }

      public Value GetItem()
      {
         using (var popper = new RegionPopper(new Region(), "get-item"))
         {
            popper.Push();
            Regions.Current.SetParameter("$", builder.Length);
            var arguments = new Array(Arguments.GetValues(builder.Length));
            var iterator = new NSIteratorByLength(arguments.GetGenerator(), builder.Length);
            var list = iterator.ToList();
            if (list.Count == 0)
               return "";
            return list.Select(getItem).Listify();
         }
      }

      public Value SetItem()
      {
         var popped = Arguments.Values.Pop();
         if (popped.IsNone)
            return this;

         var value = popped.Value.Element.AssignmentValue();
         var values = popped.Value.Array;
         using (var popper = new RegionPopper(new Region(), "set-item"))
         {
            popper.Push();
            Regions.Current.SetParameter("$", builder.Length);
            var arguments = new Array(values);
            var iterator = new NSIteratorByLength(arguments.GetGenerator(), builder.Length);
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
         if (i.Between(0).Until(builder.Length))
            return builder[i].ToString();
         return "";
      }

      void setItem(Value index, string value)
      {
         var i = index.Int;
         var k = 0;
         for (var j = i; j < value.Length && j < builder.Length; j++)
            builder[j] = value[k++];
      }

      public override string ToString() => $"{{'{Text}'}}";
   }
}