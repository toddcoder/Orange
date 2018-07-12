using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static System.Math;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Values.Object;
using static Orange.Library.Values.Object.VisibilityType;
using static System.Linq.Enumerable;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;
using static Orange.Library.Verbs.MultiAssign;

namespace Orange.Library.Values
{
   public class Parameter
   {
      public static Parameter FromComparisand(int index, Block comparisand, Block condition) => new Parameter($"${index}",
         PushValue(new Nil()), comparisand: comparisand, condition: condition);

      public Parameter(string name, Block defaultValue = null, VisibilityType visibility = Public,
         bool readOnly = false, bool lazy = false, Block comparisand = null, Block condition = null)
      {
         Name = name;
         DefaultValue = defaultValue ?? new Block();
         Visibility = visibility;
         ReadOnly = readOnly;
         Lazy = lazy;
         Comparisand = comparisand;
         PlaceholderName = "";
         Condition = condition;
      }

      public string Name { get; }

      public Block DefaultValue { get; }

      public VisibilityType Visibility { get; }

      public bool ReadOnly { get; }

      public bool Lazy { get; }

      public Block Comparisand { get; }

      public string PlaceholderName { get; set; }

      public Block Condition { get; }

      public override string ToString() => Name;

      public Parameter Clone() => new Parameter(Name.Copy(), (Block)DefaultValue?.Clone(), Visibility, ReadOnly, Lazy,
         Comparisand, (Block)Condition?.Clone());

      public void Assign(Value value, bool readOnly, bool setting, bool _override = false)
      {
         if (setting)
            Regions[Name] = ConvertIfNumeric(value);
         else
            Regions.SetParameter(Name, ConvertIfNumeric(value), _override: _override);
         if (readOnly)
            Regions.SetReadOnly(Name);
      }
   }

   public class ParameterValue
   {
      static Value evaluateBlock(Block block, bool lazy)
      {
         if (lazy)
            return new Thunk(block, Regions.Parent());

         var value = block.Evaluate();
         value = value.ArgumentValue();
         return ConvertIfNumeric(value);
      }

      public ParameterValue(string name, Block block, VisibilityType visibility, bool lazy)
      {
         Name = name;
         Value = evaluateBlock(block, lazy);
         Bound = BoundValue.Unbind(Value, out var boundName, out var innerValue);
         if (Bound)
         {
            BoundName = boundName;
            Value = innerValue;
         }
         else
            BoundName = "";
         Visibility = visibility;
         DefaultValue = PushValue("");
      }

      public ParameterValue(string name, Value value, VisibilityType visibility)
      {
         Name = name;
         Value = value.ArgumentValue();
         Bound = BoundValue.Unbind(Value, out var boundName, out var innerValue);
         if (Bound)
         {
            BoundName = boundName;
            Value = innerValue;
         }
         else
            BoundName = "";
         Visibility = visibility;
         DefaultValue = PushValue("");
      }

      public ParameterValue Unbound() => Bound ? new ParameterValue(BoundName, Value, Visibility) : this;

      public string Name { get; set; }

      public Value Value { get; set; }

      public VisibilityType Visibility { get; set; }

      public string BoundName { get; set; }

      public bool Bound { get; set; }

      public string TrueName => Bound ? BoundName : Name;

      public Block DefaultValue { get; set; }
   }

   public class Parameters : Value
   {
      const string LOCATION = "Parameters";

      Parameter[] parameters;

      public Parameters(IEnumerable<Parameter> parameters) => this.parameters = parameters.ToArray();

      public Parameters(Array array) => parameters = array.Select(i => new Parameter(i.Value.Text)).ToArray();

      public Parameters() => parameters = new Parameter[0];

      public Parameters(string[] fieldNames) => parameters = fieldNames.Select(fn => new Parameter(fn)).ToArray();

      public Parameter this[int index]
      {
         get => parameters.Of(index, null);
         set
         {
            if (index.Between(0).Until(parameters.Length))
               parameters[index] = value;
         }
      }

      public Pattern Pattern { get; set; }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Parameters;

      public override bool IsTrue => false;

      public override Value Clone() => new Parameters(parameters.Select(p => p.Clone()).ToArray())
      {
         Pattern = (Pattern)Pattern?.Clone()
      };

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "assign", v => ((Parameters)v).Assign());
         manager.RegisterMessage(this, "map", v => ((Parameters)v).Map());
      }

      public Value Assign()
      {
         var self = Regions["self"];
         Reject(self.IsEmpty, LOCATION, "Must assign within a method or with block");
         foreach (var variableName in VariableNames)
         {
            var value = MessagingState.Send(self, variableName, new Arguments());
            if (value is Variable variable)
               variable.Value = Regions[variableName];
         }

         return null;
      }

      public Value Map() => new Lambda(Regions.Current, Arguments.Executable, this, false);

      public override string ToString() => Pattern?.ToString() ?? parameters.Listify();

      public string VariableName(int index, string defaultValue = "") => parameters.Of(index).FlatMap(p => p.Name, () => defaultValue);

      public virtual bool Usable => true;

      public int Length => parameters.Length;

      public string[] VariableNames => parameters.Select(p => p.Name).ToArray();

      static Value evaluateBlock(Block block, bool lazy)
      {
         if (lazy)
            return new Thunk(block, Regions.Parent());

         var value = block.Evaluate();
         if (value.IsArray)
            value = value.SourceArray;
         return ConvertIfNumeric(value);
      }

      public List<ParameterValue> GetArguments(Arguments arguments)
      {
         var list = new List<ParameterValue>();
         var blocks = arguments.Blocks;
         var length = blocks.Length;
         var parametersLength = parameters.Length;
         var minLength = Min(length, parametersLength);
         var visibility = parameters.ToHash(p => p.Name, p => p.Visibility).ToAutoHash(Public);
         var bound = new Hash<string, Value>();

         for (var i = 0; i < minLength; i++)
         {
            var parameter = parameters[i];
            var name = parameter.Name;
            var value = new ParameterValue(name, blocks[i], visibility[name], parameter.Lazy);
            if (value.Bound)
            {
               bound[value.BoundName] = value.Value;
               value.Value = evaluateBlock(parameter.DefaultValue, parameter.Lazy);
            }
            list.Add(value);
         }

         if (length > minLength && minLength > 0)
         {
            var lastIndex = minLength - 1;
            var name = parameters[lastIndex].Name;
            var lastValue = list[lastIndex].Value;
            var array = new Array { lastValue };
            for (var i = minLength; i < length; i++)
               array.Add(evaluateBlock(blocks[i], false));

            var value = new ParameterValue(name, array, visibility[name]);
            if (value.Bound)
            {
               bound[value.BoundName] = value.Value;
               value.Value = "";
            }
            list.Add(value);
         }
         else if (parametersLength > minLength)
            for (var i = minLength; i < parametersLength; i++)
            {
               var parameter = parameters[i];
               var name = parameter.Name;
               var value = new ParameterValue(name, parameter.DefaultValue, visibility[name], parameter.Lazy);
               if (value.Bound)
                  bound[value.BoundName] = value.Value;
               list.Add(value);
            }

         foreach (var parameterValue in list)
         {
            var value = bound[parameterValue.Name];
            if (value != null)
               parameterValue.Value = value;
         }

         return list;
      }

      public static void SetArguments(List<ParameterValue> list)
      {
         foreach (var parameterValue in list)
            Regions.SetParameter(parameterValue.Name, parameterValue.Value.ArgumentValue(),
               parameterValue.Visibility);
      }

      public bool Splatting { get; set; }

      public bool Multi { get; set; }

      public override bool IsArray => true;

      public override Value SourceArray
      {
         get
         {
            var array = new Array();
            foreach (var parameter in parameters)
            {
               var valueGraph = new ValueGraph(parameter.Name);
               var nameGraph = new ValueGraph(parameter.Name) { Value = parameter.Name };
               var defaultGraph = new ValueGraph("default") { Value = parameter.DefaultValue };
               var isLocalGraph = new ValueGraph("visibility") { Value = parameter.Visibility.ToString() };

               valueGraph["name"] = nameGraph;
               valueGraph["default"] = defaultGraph;
               valueGraph["visibility"] = isLocalGraph;
               array[parameter.Name] = new Graph(valueGraph);
            }

            return array;
         }
      }

      public bool XMethod => VariableNames.Length > 0 && VariableNames[0].IsMatch("^ 'self' | 'class' $");

      public void Unshift(Parameter parameter)
      {
         var newParameters = new Parameter[parameters.Length + 1];
         parameters.CopyTo(newParameters, 1);
         newParameters[0] = parameter;
         parameters = newParameters;
      }

      public void Unshift(Parameters extraParameters)
      {
         var newParameters = new Parameter[parameters.Length + extraParameters.Length];
         parameters.CopyTo(newParameters, extraParameters.Length);
         for (var i = 0; i < extraParameters.Length; i++)
            newParameters[i] = extraParameters[i];

         parameters = newParameters;
      }

      public void Append(Parameters additional)
      {
         var length = parameters.Length;
         var newParameters = new Parameter[length + additional.Length];
         parameters.CopyTo(newParameters, 0);
         for (var i = 0; i < additional.Length; i++)
            newParameters[i + length] = additional[i];

         parameters = newParameters;
      }

      public Parameter[] GetParameters() => parameters;

      public bool AnyComparisands => parameters.Any(p => p.Comparisand != null);

      public Block Condition { get; set; }

      public void SetValues(Value value, int index = -1)
      {
         if (Length == 1 && this[0].Name == "$_")
         {
            awkify(value);
            return;
         }

         var current = Regions.Current;

         if ((value is OTuple || value is Array) && Length > 1)
         {
            Splat(value, this, true, false, true);
            return;
         }

         var names = parameters.Select(p => p.Name).ToArray();
         if (value is KeyedValue keyValue)
         {
            var key = keyValue.Key;
            value = keyValue.Value;
            switch (names.Length)
            {
               case 0:
                  break;
               case 1:
                  current.SetParameter(names[0], value, _override: true);
                  break;
               case 2:
                  current.SetParameter(names[0], value, _override: true);
                  current.SetParameter(names[1], key, _override: true);
                  break;
               case 3:
                  current.SetParameter(names[0], value, _override: true);
                  current.SetParameter(names[1], key, _override: true);
                  current.SetParameter(names[2], index, _override: true);
                  break;
            }
         }
         else
         {
            switch (names.Length)
            {
               case 0:
                  break;
               case 1:
                  current.SetParameter(names[0], value, _override: true);
                  break;
               case 2:
                  current.SetParameter(names[0], value, _override: true);
                  current.SetParameter(names[1], index, _override: true);
                  break;
            }
         }
      }

      static void awkify(Value value)
      {
         var newParameters = new Parameters(Range(0, 10).Select(i => new Parameter($"${i}")));
         Splat(value, newParameters, true, false, true);
         var current = Regions.Current;
         for (var i = 0; i < 10; i++)
         {
            var name = $"${i}";
            if (!Regions.VariableExists(name))
               current.SetParameter(name, "");
         }
      }
   }
}