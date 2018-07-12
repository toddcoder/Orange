using System;
using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library
{
   public class ParameterAssistant : IDisposable
   {
      const string LOCATION = "Parameter assistant";

      public enum SignalType
      {
         None,
         Breaking,
         Continuing,
         ReturningNull
      }

      Arguments arguments;
      DefaultParameterNames names;
      List<string> unpackedVariables;
      bool splatting;
      bool multi;
      Value comparisand;

      public ParameterAssistant(Arguments arguments, bool useUpperLevel = false)
      {
         RejectNull(arguments, LOCATION, "Arguments not passed");
         this.arguments = arguments;
         names = useUpperLevel ? State.PushUpperLevelParameterNames() : State.PushDefaultParameterNames();
         unpackedVariables = new List<string>();
         splatting = arguments.Splatting;

         multi = arguments.Parameters.Multi;
         if (!multi)
            return;

         Assert(arguments.Parameters.Length > 0, LOCATION, "You must have at least one parameter");
         comparisand = arguments.Parameters[0].Comparisand.Evaluate();
      }

      public ParameterAssistant(ParameterBlock parameterBlock, bool useUpperLevel = false)
         : this(new Arguments(parameterBlock), useUpperLevel) => splatting = parameterBlock.Splatting;

      public Block Block(bool returnBlock = true)
      {
         if (arguments.Executable == null || !arguments.Executable.CanExecute)
            return null;

         var block = returnBlock ? new ReturnBlock(arguments.Executable) : arguments.Executable;
         return block.CanExecute ? block : null;
      }

      public Arguments Arguments => arguments;

      public Value NilOrClosure => (Value)arguments.Lambda ?? new Nil();

      public DefaultParameterNames Names => names;

      public void ArrayParameters()
      {
         if (splatting)
            return;

         names.ValueVariable = arguments.VariableName(0, names.ValueVariable);
         names.KeyVariable = arguments.VariableName(1, names.KeyVariable);
         names.IndexVariable = arguments.VariableName(2, names.IndexVariable);
         getUnpackedVariables();
      }

      void getUnpackedVariables()
      {
         for (var i = 3; i < arguments.Parameters.Length; i++)
            unpackedVariables.Add(arguments.Parameters[i].Name);
      }

      public void LoopParameters()
      {
         names.ValueVariable = arguments.VariableName(0, names.ValueVariable);
         names.IndexVariable = arguments.VariableName(1, names.IndexVariable);
         names.NumberVariable = arguments.VariableName(2, names.NumberVariable);
      }

      public void TwoValueParameters()
      {
         names.ValueVariable1 = arguments.VariableName(0, names.ValueVariable1);
         names.ValueVariable2 = arguments.VariableName(1, names.ValueVariable2);
      }

      public void MergeParameters()
      {
         names.ValueVariable1 = arguments.VariableName(0, names.ValueVariable1);
         names.ValueVariable2 = arguments.VariableName(1, names.ValueVariable2);
         names.KeyVariable = arguments.VariableName(2, names.KeyVariable);
      }

      public void BreakOnParameters()
      {
         names.ArrayVariable = arguments.VariableName(0, names.ArrayVariable);
         names.KeyVariable = arguments.VariableName(1, names.KeyVariable);
         names.IndexVariable = arguments.VariableName(2, names.IndexVariable);
      }

      public void IteratorParameter()
      {
         names.IterVariable = arguments.VariableName(0, names.IterVariable);
      }

      public void ForParameters()
      {
         names.ValueVariable = arguments.VariableName(0, names.ValueVariable);
         names.IndexVariable = arguments.VariableName(1, names.IndexVariable);
      }

      public void IteratorParameter(Parameters parameters)
      {
         if (parameters == null || parameters.Length == 0)
            IteratorParameter();
         else
            names.IterVariable = parameters.VariableName(0, names.IterVariable);
      }

      public void ReplacementParameters()
      {
         names.ValueVariable = arguments.VariableName(0, names.ValueVariable);
         names.PositionVariable = arguments.VariableName(1, names.PositionVariable);
         names.LengthVariable = arguments.VariableName(2, names.LengthVariable);
         names.GroupVariable = arguments.VariableName(3, names.GroupVariable);
      }

      public void ReaderParameter()
      {
         names.ReaderVariable = arguments.VariableName(0, names.ReaderVariable);
      }

      public void WriterParameter()
      {
         names.WriterVariable = arguments.VariableName(0, names.WriterVariable);
      }

      void setUnpacked(Value value)
      {
         if (names.UnpackedVariables.Count == 0 && unpackedVariables.Count == 0)
            return;

         var array = value.IsArray ? (Array)value.SourceArray : new Array(State.FieldPattern.Split(value.Text));
         if (names.UnpackedVariables.Count > 0)
            Assign.FromFieldsLocal(array, names.UnpackedVariables.ToArray());
         if (unpackedVariables.Count > 0)
            Assign.FromFieldsLocal(array, unpackedVariables.ToArray());
      }

      void caseMatch(Value value)
      {
         Assert(Case.Match(value, comparisand, Regions.Current, false, false, null), LOCATION, "Comparison must match");
      }

      public void SetParameterValues(Array.IterItem item)
      {
         if (splatting)
         {
            var source = item.Value.IsArray ? (Array)item.Value.SourceArray :
               new Array(State.FieldPattern.Split(item.Value.Text));
            Assign.FromFieldsLocal(source, arguments.Parameters);
         }
         else if (multi)
            caseMatch(item.Value);
         else
         {
            Regions.SetLocal(names.ValueVariable, item.Value);
            setUnpacked(item.Value);
            Regions.SetLocal(names.KeyVariable, item.Key);
            Regions.SetLocal(names.IndexVariable, item.Index);
         }
      }

      public void SetParameterValues(Value value, string key, int index)
      {
         if (splatting)
         {
            var source = value.IsArray ? (Array)value.SourceArray :
               new Array(State.FieldPattern.Split(value.Text));
            Assign.FromFieldsLocal(source, arguments.Parameters);
         }
         else if (multi)
            caseMatch(value);
         else
         {
            Regions.SetLocal(names.ValueVariable, value);
            setUnpacked(value);
            Regions.SetLocal(names.KeyVariable, key);
            Regions.SetLocal(names.IndexVariable, index);
         }
      }

      public void SetParameterValues(Value value, Value value2)
      {
         Regions.SetLocal(names.ValueVariable1, value);
         Regions.SetLocal(names.ValueVariable2, value2);
      }

      public void SetLoopParameters(string line, int index)
      {
         Regions.SetLocal(names.ValueVariable, line);
         Regions.SetLocal(names.IndexVariable, index);
         Regions.SetLocal(names.NumberVariable, index + 1);
      }

      public void SetMergeParameters(string key, Value value, Value value2)
      {
         Regions.SetLocal(names.ValueVariable1, value);
         Regions.SetLocal(names.ValueVariable2, value2);
         Regions.SetLocal(names.KeyVariable, key);
      }

      public static void SetMultipleParameters(Parameters parameters, Array array)
      {
         Assign.FromFieldsLocal(array, parameters, true);
      }

      public void SetBreakOnParameters(Array array, string key, int index)
      {
         Regions.SetLocal(names.ArrayVariable, array);
         Regions.SetLocal(names.KeyVariable, key);
         Regions.SetLocal(names.IndexVariable, index);
      }

      public void SetIteratorParameter(Value value)
      {
         Regions.SetLocal(names.IterVariable, value);
      }

      public void SetFor(Value value, int index)
      {
         Regions.SetLocal(names.ValueVariable, value);
         Regions.SetLocal(names.IndexVariable, index);
      }

      public void SetReplacement(string value, int position, int length, int groupIndex)
      {
         Regions.SetLocal(names.ValueVariable, value);
         Regions.SetLocal(names.PositionVariable, position);
         Regions.SetLocal(names.LengthVariable, length);
         Regions.SetLocal(names.GroupVariable, groupIndex);
      }

      public void SetReplacement()
      {
         var value = arguments[0].Text;
         var position = arguments[1].Int;
         var length = arguments[2].Int;
         var groupIndex = arguments[3].Int;
         SetReplacement(value, position, length, groupIndex);
      }

      public static SignalType Signal()
      {
         if (State.ExitSignal)
         {
            State.ExitSignal = false;
            State.LateBlock = null;
            return Breaking;
         }

         if (State.SkipSignal)
         {
            State.SkipSignal = false;
            State.LateBlock = null;
            return Continuing;
         }

         if (State.ReturnSignal)
         {
            State.LateBlock = null;
            return ReturningNull;
         }

         return SignalType.None;
      }

      public static void Last()
      {
         var lateBlock = State.LateBlock;
         if (lateBlock == null || !lateBlock.CanExecute)
            return;

         lateBlock.Evaluate();
         State.LateBlock = null;
      }

      public void Dispose()
      {
         Last();
         State.PopDefaultParameterNames();
      }
   }
}