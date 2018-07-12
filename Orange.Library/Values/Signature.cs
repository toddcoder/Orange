using System.Collections.Generic;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Signature : Value
   {
      string name;
      int parameterCount;
      bool optional;

      public Signature(string name, int parameterCount, bool optional)
      {
         this.name = name;
         this.parameterCount = parameterCount;
         this.optional = optional;
      }

      public string Name => name;

      public int ParameterCount => parameterCount;

      public bool Optional => optional;

      public override int Compare(Value value)
      {
         if (value is Signature other)
            return name.CompareTo(other.Name) + parameterCount.CompareTo(other.parameterCount) + optional.CompareTo(other.optional);

         return -1;
      }

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Signature;

      public override bool IsTrue => false;

      public override Value Clone() => new Signature(name, parameterCount, optional);

      protected override void registerMessages(MessageManager manager) { }

      public override string ToString() => (optional ? "optional " : "") + $"{name}({parameters()})";

      string parameters()
      {
         var parameter = "a";
         var list = new List<string>();
         for (var i = 0; i < parameterCount; i++)
         {
            list.Add(parameter);
            parameter = parameter.Succ();
         }

         return list.Listify();
      }

      public string UnmangledSignature => $"{Unmangle(name)}({parameters()})";
   }
}