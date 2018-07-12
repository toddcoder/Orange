using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orange.Library.Managers;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static System.Math;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class StringIndexer : Variable
   {
      String text;
      Block indexesBlock;

      public StringIndexer(String text, Block indexesBlock)
         : base(VAR_ANONYMOUS + CompilerState.ObjectID())
      {
         this.text = text;
         this.indexesBlock = indexesBlock;
      }

      public override Value Value
      {
         get
         {
            var result = new StringBuilder();
            var length = text.Text.Length;
            foreach (var index in getIndexes())
            {
               var correctedIndex = index;
               if (correctedIndex < 0)
                  correctedIndex = WrapIndex(correctedIndex, length, true);
               if (correctedIndex >= length)
                  result.Append("");
               else
                  result.Append(text.Text[correctedIndex]);
            }

            return result.ToString();
         }
         set
         {
            var indexes = getIndexes();
            if (indexes.Length == 0)
               return;

            Slicer slicer = text.Text;
            if (indexes.Length == 1)
            {
               var correctedIndex = indexes[0];
               if (correctedIndex < 0)
                  correctedIndex = WrapIndex(correctedIndex, slicer.Length, true);
               slicer[correctedIndex, 1] = value.Text;
            }
            else if (inRange())
            {
               var start = indexes[0];
               var length = indexes.Length;
               slicer[start, length] = value.Text;
            }
            else
            {
               var valueText = value.Text;
               if (valueText.Length <= 1)
                  foreach (var index in indexes)
                     slicer[index, 1] = valueText;
               else
               {
                  var minLength = Math.Min(slicer.Length, indexes.Length);
                  for (var i = 0; i < minLength; i++)
                     slicer[i, 1] = valueText.Skip(i).Take(1);
               }
            }

            text.Text = slicer.ToString();
         }
      }

      bool inRange()
      {
         var offset = none<int>();
         var indexes = getIndexes();
         for (var i = 0; i < indexes.Length; i++)
            if (offset.IsNone)
               offset = (indexes[i] - i).Some();
            else if (indexes[i] - offset.Value != i)
               return false;

         return true;
      }

      void setLength()
      {
         var length = text.Text.Length;
         foreach (var wrapping in indexesBlock.OfType<IWrapping>())
         {
            wrapping.SetLength(length);
            wrapping.IsSlice = true;
         }
      }

      int[] getIndexes()
      {
         setLength();
         var result = indexesBlock.Evaluate();
         if (result == null)
            return new int[0];

         //var range = result.As<IRange>();
         if (result is IRange range)
         {
            var length = text.Text.Length;
            var start = (int)range.Start.Number;
            if (start < 0)
               start = WrapIndex(start, length, true);
            var stop = (int)range.Stop.Number;
            if (stop < 0)
               stop = WrapIndex(stop, length, true);
            var increment = Abs((int)range.Increment.Number);
            var array = new List<int>();
            if (start <= stop)
               for (var i = start; i <= stop; i += increment)
               {
                  var value = i;
                  if (value < 0)
                     value = WrapIndex(value, length, true);
                  array.Add(value);
               }
            else
               for (var i = start; i >= stop; i -= increment)
               {
                  var value = i;
                  if (value < 0)
                     value = WrapIndex(value, length, true);
                  array.Add(value);
               }

            return array.ToArray();
         }

         return result.IsArray ? ((Array)result.SourceArray).Values.Select(v => (int)v.Number).ToArray() : new[] { (int)result.Number };
      }

      protected override void registerMessages(MessageManager manager)
      {
         base.registerMessages(manager);
         manager.RegisterMessage(this, "each", v => ((StringIndexer)v).Each());
         manager.RegisterMessage(this, "remove", v => ((StringIndexer)v).Remove());
         manager.RegisterMessage(this, "insert", v => ((StringIndexer)v).Insert());
      }

      public Value Insert()
      {
         var indexes = getIndexes();
         var target = text.Text;
         var source = Arguments[0].Text;
         target = indexes.Aggregate(target, (s, i) => s.Insert(WrapIndex(i, target.Length, true), source));
         text.Text = target;
         return this;
      }

      public Value Remove()
      {
         var indexes = getIndexes();
         Slicer slicer = text.Text;
         foreach (var index in indexes)
         {
            var correctedIndex = index;
            if (correctedIndex < 0)
               correctedIndex = WrapIndex(correctedIndex, text.Text.Length, true);
            slicer[correctedIndex, 1] = "";
         }

         text.Text = slicer.ToString();
         return this;
      }

      public override string ContainerType => ValueType.StringIndexer.ToString();

      public Value Each()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return null;

            RegionManager.Regions.Push("string-indexer");

            var changes = new Hash<int, string>();
            var indexes = getIndexes();

            foreach (var index in indexes)
            {
               var correctedIndex = WrapIndex(index, text.Text.Length, true);
               assistant.SetParameterValues(text.Text.Skip(index).Take(1), index.ToString(), correctedIndex);
               var result = block.Evaluate();
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;

               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return null;
               }

               if (result != null)
                  changes[correctedIndex] = result.Text;
            }

            Slicer slicer = text.Text;
            foreach (var item in changes)
               slicer[item.Key] = item.Value;

            text.Text = slicer.ToString();

            RegionManager.Regions.Pop("string-indexer");

            return null;
         }
      }

      public override bool IsIndexer => true;

      public override Value MessageTarget(string message)
      {
         switch (message)
         {
            case "each":
            case "remove":
            case "insert":
               return this;
            default:
               return Value;
         }
      }

      public override Value AlternateValue(string message) => Value;
   }
}