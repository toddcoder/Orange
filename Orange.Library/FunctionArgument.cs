using Orange.Library.Values;

namespace Orange.Library
{
   public class FunctionArgument
   {
      public static FunctionArgument Create(Value value)
      {
         if (value.Type == Value.ValueType.KeyedValue)
         {
            var hashKey = (KeyedValue)value;
            return new FunctionArgument { Name = hashKey.Key, DefaultValue = hashKey.Value.Clone() };
         }

         if (value.IsVariable)
         {
            var variable = (Variable)value;
            return new FunctionArgument { Name = variable.Name, DefaultValue = new String("") };
         }

         return new FunctionArgument { Name = value.Text, DefaultValue = new String("") };
      }

      public string Name { get; set; }

      public Value DefaultValue { get; set; }

      public Value Get(Value value) => value == null ? DefaultValue : value.ArgumentValue();

      public override string ToString() => DefaultValue == null ? Name : $"{Name}=>{DefaultValue}";
   }
}