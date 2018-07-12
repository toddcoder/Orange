using System;
using Orange.Library.Managers;
using Standard.Types.Strings;
using System.Linq;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Values
{
   public class Format : Value
   {
      Parameters parameters;
      IStringify stringify;

      public Format(Parameters parameters, IStringify stringify)
      {
         this.parameters = parameters;
         this.stringify = stringify;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            return ToString();
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

      public override ValueType Type => ValueType.Format;

      public override bool IsTrue => false;

      public override Value Clone() => new Format((Parameters)parameters.Clone(), (IStringify)((Value)stringify).Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((Format)v).Apply());
         manager.RegisterMessage(this, "invoke", v => ((Format)v).Invoke());
      }

      public Value Apply()
      {
         var argument = Arguments.ApplyValue;
         Regions.Push("format");
         var array = argument.IsArray ? (Array)argument.SourceArray : new Array { argument };
         var variableNames = parameters.VariableNames;
         var result = text(variableNames, array.Values.Select(v => v.Text).ToArray());
         Regions.Pop("format");
         return result;
      }

      Value text(string[] parameters, string[] arguments)
      {
         var minLength = Math.Min(parameters.Length, arguments.Length);
         for (var i = 0; i < minLength; i++)
            Regions.SetLocal(parameters[i], arguments[i]);
         var result = stringify.String.Text;

         return result;
      }

      public Value Invoke()
      {
         Regions.Push("format");
         var result = text(parameters.VariableNames, Arguments.Values.Select(v => v.Text).ToArray());
         Regions.Pop("format");
         return result;
      }

      public override string ToString() => $"{parameters} ? {stringify}";

      public Parameters Parameters => parameters;

      public IStringify Stringify => stringify;
   }
}