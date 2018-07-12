using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using static System.Math;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Assign : Verb
   {
      const string LOCATION = "Assign";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var value = stack.Pop(true, LOCATION);
         var target = stack.Pop(false, LOCATION);
         if (target is Parameters parameters)
         {
            if (value is Object obj)
               return fromObject(parameters, obj);

            return value.IsArray ? fromFields(target, value.SourceArray) : fromText(target, value);
         }

         if (target.IsArray)
            return value.IsArray ? fromArray(target, value.SourceArray) : fromArrayText(target, value);

         if (target is Variable variable)
         {
            value.AssignmentValue().AssignTo(variable);
            return variable;
         }

         Throw(LOCATION, "Expected a variable");
         return null;
      }

      static Array fromFields(Value target, Value value)
      {
         var parameters = (Parameters)target.Resolve();
         var sourceArray = (Array)value.Resolve();
         return FromFields(sourceArray, parameters);
      }

      static Array fromArray(Value target, Value value)
      {
         var parameters = new Parameters((Array)target.SourceArray);
         var sourceArray = (Array)value.Resolve();
         return FromFields(sourceArray, parameters);
      }

      static Array fromArrayText(Value target, Value value)
      {
         var parameters = new Parameters((Array)target.SourceArray);
         return fromText(parameters, value);
      }

      public static Array FromFields(Array sourceArray, Parameters parameters, bool readOnly = false)
      {
         var length = sourceArray.Length;
         var parametersLength = parameters.Length;
         var minLength = Min(length, parametersLength);
         for (var i = 0; i < minLength; i++)
         {
            var parameter = parameters[i];
            var variableName = parameter.Name;
            Regions.CreateVariableIfNonexistant(variableName);
            Regions[variableName] = ConvertIfNumeric(sourceArray[i]);
         }

         if (length > minLength)
         {
            if (minLength == 0)
               return new Array();

            var parameter = parameters[minLength - 1];
            var variableName = parameter.Name;
            var lastValue = Regions[variableName];
            var array = new Array { lastValue };
            for (var i = minLength; i < length; i++)
               array.Add(ConvertIfNumeric(sourceArray[i]));

            Regions.CreateVariableIfNonexistant(variableName);
            Regions[variableName] = array;
            if (parameter.ReadOnly)
               Regions.SetReadOnly(variableName);
         }
         else if (parametersLength > minLength)
            for (var i = minLength; i < parametersLength; i++)
            {
               var parameter = parameters[i];
               var variableName = parameter.Name;
               Regions.CreateVariableIfNonexistant(variableName);
               Regions[variableName] = ConvertIfNumeric(parameter.DefaultValue?.Evaluate() ?? "");
               if (parameter.ReadOnly)
                  Regions.SetReadOnly(variableName);
            }

         var result = new Array();
         for (var i = 0; i < parametersLength; i++)
         {
            var name = parameters[i].Name;
            result[name] = Regions[name];
         }

         return result;
      }

      public static Array FromFieldsLocal(Array sourceArray, Parameters parameters, bool keepOneItemAsArray = false)
      {
         var length = sourceArray.Length;
         var parametersLength = parameters.Length;
         if (length == 1 && parametersLength == 1)
         {
            var variableName = parameters[0].Name;
            Regions.SetLocal(variableName, sourceArray[0]);
            if (parameters[0].ReadOnly)
               Regions.SetReadOnly(variableName);
            return sourceArray;
         }

         var minLength = Min(length, parametersLength);
         for (var i = 0; i < minLength; i++)
         {
            var parameter = parameters[i];
            var variable = parameter.Name;
            Regions.SetLocal(variable, ConvertIfNumeric(sourceArray[i]));
            if (parameter.ReadOnly)
               Regions.SetReadOnly(variable);
         }

         if (length > minLength)
         {
            var parameter = parameters[minLength - 1];
            if (parameter != null)
            {
               var variableName = parameter.Name;
               var lastValue = Regions[variableName];
               var array = new Array { lastValue };
               for (var i = minLength; i < length; i++)
                  array.Add(ConvertIfNumeric(sourceArray[i]));

               Regions.SetLocal(variableName, array);
               if (parameter.ReadOnly)
                  Regions.SetReadOnly(variableName);
            }
         }
         else if (parametersLength > minLength)
            for (var i = minLength; i < parametersLength; i++)
            {
               var parameter = parameters[i];
               var variable = parameter.Name;
               Regions.SetLocal(variable, ConvertIfNumeric(parameter.DefaultValue?.Evaluate() ?? ""));
               if (parameter.ReadOnly)
                  Regions.SetReadOnly(variable);
            }

         var result = new Array();
         for (var i = 0; i < parametersLength; i++)
         {
            var name = parameters[i].Name;
            result[name] = Regions[name];
         }

         return result;
      }

      public static void FromFieldsLocal(Array sourceArray, string[] parameters, bool keepOneItemAsArray = false)
      {
         var length = sourceArray.Length;
         var parametersLength = parameters.Length;
         if (length == 1 && parametersLength == 1)
         {
            Regions.SetLocal(parameters[0], sourceArray);
            return;
         }

         var minLength = Min(length, parametersLength);
         for (var i = 0; i < minLength; i++)
            Regions.SetLocal(parameters[i], ConvertIfNumeric(sourceArray[i]));

         if (length > minLength)
         {
            var variableName = parameters[minLength - 1];
            var lastValue = Regions[variableName];
            var array = new Array { lastValue };
            for (var i = minLength; i < length; i++)
               array.Add(ConvertIfNumeric(sourceArray[i]));

            Regions.SetLocal(variableName, array);
         }
         else if (parametersLength > minLength)
            for (var i = minLength; i < parametersLength; i++)
               Regions.SetLocal(parameters[i], "");

         var result = new Array();
         for (var i = 0; i < parametersLength; i++)
         {
            var name = parameters[i];
            result[name] = Regions[name];
         }
      }

      static Array fromText(Value target, Value value)
      {
         var fields = State.FieldPattern.Split(value.Text);
         var fieldQueue = new Queue<string>(fields);
         var parameters = (Parameters)target;
         var variableNames = parameters.VariableNames;
         var varQueue = new Queue<string>(variableNames);
         while (varQueue.Count > 0)
         {
            var variableName = varQueue.Dequeue();
            var field = fieldQueue.Count > 0 ? fieldQueue.Dequeue() : "";
            if (varQueue.Count == 0 && fieldQueue.Count > 0)
            {
               var fieldList = new List<string> { field };
               while (fieldQueue.Count > 0)
                  fieldList.Add(fieldQueue.Dequeue());

               field = fieldList.Listify(State.FieldSeparator.Text);
            }

            Regions.CreateVariableIfNonexistant(variableName);
            Regions[variableName] = ConvertIfNumeric(field);
         }

         return new Array(fields);
      }

      static Array fromObject(Parameters target, Object source)
      {
         var array = new Array();
         foreach (var variableName in target.VariableNames)
            if (SendMessage(source, variableName) is Variable variable)
            {
               var value = variable.Value;
               Regions.CreateVariableIfNonexistant(variableName);
               Regions[variableName] = value;
               array[variableName] = value;
            }

         return array;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => "=";

      public override bool LeftToRight => false;
   }
}