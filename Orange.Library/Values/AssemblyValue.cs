using System.Reflection;
using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class AssemblyValue : Value
   {
      Assembly assembly;

      public AssemblyValue(string description)
      {
         if (description.IsEmpty())
            description = "mscorlib";
         assembly = Assembly.Load(description);
      }

      public AssemblyValue(Assembly assembly) => this.assembly = assembly;

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            return assembly.FullName;
         }
         set
         {
         }
      }

      public override double Number
      {
         get
         {
            return 0;
         }
         set
         {
         }
      }

      public override ValueType Type => ValueType.Assembly;

      public override bool IsTrue => true;

      public override Value Clone() => new AssemblyValue(assembly);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "type", v => ((AssemblyValue)v).TypeFromAssembly());
      }

      public Value TypeFromAssembly()
      {
         var typeName = Arguments[0].Text;
         return new TypeValue(assembly, typeName);
      }
   }
}