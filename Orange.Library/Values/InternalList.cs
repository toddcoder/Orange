using System.Linq;
using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Strings;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterBlock;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Parameters;

namespace Orange.Library.Values
{
   public class InternalList : Value, IMessageHandler
   {
      Hash<string, Value> fields;
      string anonymousName;

      public InternalList()
      {
         fields = new AutoHash<string, Value>
         {
            Default = DefaultType.Value,
            DefaultValue = ""
         };
         anonymousName = "a";
      }

      public InternalList(AutoHash<string, Value> fields) => this.fields = fields;

      public Value this[string fieldName]
      {
         get => fields[fieldName];
         set => fields[fieldName] = value;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            return fields.ValueArray().Listify(State.FieldSeparator.Text);
         }
         set
         {
         }
      }

      public override double Number
      {
         get
         {
            return fields.Count;
         }
         set
         {
         }
      }

      public override ValueType Type => ValueType.InternalList;

      public override bool IsTrue => false;

      public override Value Clone()
      {
         var newFields = new AutoHash<string, Value>
         {
            Default = DefaultType.Value,
            DefaultValue = ""
         };
         foreach (var item in fields)
            newFields[item.Key] = item.Value.Clone();
         return new InternalList(newFields);
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "arr", v => ((InternalList)v).Array());
         manager.RegisterMessage(this, "assign", v => ((InternalList)v).Assign());
      }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         if (fields.ContainsKey(messageName))
         {
            handled = true;
            var possibleExecutable = fields[messageName];
            return possibleExecutable.IsExecutable ? invokeExecutable(possibleExecutable, arguments) :
               new ListField(messageName, this);
         }
         handled = false;
         return null;
      }

      Value invokeExecutable(Value value, Arguments arguments)
      {
         var parameterBlock = FromExecutable(value);
         parameterBlock.Block.AutoRegister = false;
         var values = parameterBlock.Parameters.GetArguments(arguments);
         State.RegisterBlock(parameterBlock.Block);
         Regions.SetLocal("self", this);
         SetArguments(values);
         var result = parameterBlock.Block.Evaluate();
         result = State.UseReturnValue(result);
         State.UnregisterBlock();
         return result;
      }

      public bool RespondsTo(string messageName) => fields.ContainsKey(messageName);

      public void Add(Value value)
      {
         var keyedValue = value as KeyedValue;
         if (keyedValue != null)
            this[keyedValue.Key] = keyedValue.Value;
         else
         {
            fields[anonymousName] = value;
            anonymousName = anonymousName.Succ();
         }
      }

      public void Add(string key, Value value) => fields[key] = value;

      public override string ToString() => fields.Select(i => $"{i.Key} := {i.Value}").Listify();

      public Value Array()
      {
         var array = new Array();
         foreach (var item in fields)
            array[item.Key] = item.Value;
         return array;
      }

      public Value Assign()
      {
         var array = new Array();
         foreach (var item in fields)
         {
            Regions[item.Key] = item.Value;
            array[item.Key] = item.Value;
         }
         return array;
      }
   }
}