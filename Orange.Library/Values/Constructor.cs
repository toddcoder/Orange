using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using static System.Math;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Values
{
   public class Constructor : Value, IMatch
   {
      string dataName;
      string name;
      Value[] values;

      public Constructor(string dataName, string name, Value[] values)
      {
         this.dataName = dataName;
         this.name = name;
         this.values = values;
      }

      public override int Compare(Value value) => value is Constructor c ? dataName.CompareTo(c.dataName) + name.CompareTo(c.name) : -1;

      public override string Text
      {
         get => $"{name}" + (values.Length == 0 ? "" : $"({values.Select(v => v.Text).Listify()})");
         set { }
      }

      public override double Number
      {
         get => values.Length;
         set { }
      }

      public override ValueType Type => ValueType.Constructor;

      public override bool IsTrue => true;

      public override Value Clone() => new Constructor(dataName, name, values.Select(v => v.Clone()).ToArray());

      protected override void registerMessages(MessageManager manager) { }

      public override string ToString() => $"{dataName}.{Text}";

      public bool Match(Value comparisand)
      {
         switch (comparisand)
         {
            case Any _:
               return true;
            case Placeholder placeholder:
               var bindingName = placeholder.Text;
               Regions.SetBinding(bindingName, this, false);
               return true;
            case Constructor constructor when name == constructor.name && dataName == constructor.dataName:
               for (var i = 0; i < Min(values.Length, constructor.values.Length); i++)
               {
                  var left = values[i];
                  var right = constructor.values[i];
                  if (!Case.Match(left, right, true, new Boolean(true).Block))
                     return false;
               }

               return true;
            default:
               return false;
         }
      }
   }
}