using System.Collections.Generic;
using Orange.Library.Managers;
using Standard.Types.Arrays;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Records : Value
   {
      public class Range
      {
         public int StartIndex { get; set; }

         public int StopIndex { get; set; }
      }

      String text;
      Range[] ranges;

      public Records(String text, Range[] ranges)
      {
         this.text = text;
         this.ranges = ranges;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => text.Text;
         set => text.Text = value;
      }

      public override double Number
      {
         get => text.Number;
         set => text.Number = value;
      }

      public override ValueType Type => ValueType.Records;

      public override bool IsTrue => false;

      public bool Packed { get; set; }

      public override Value Clone() => new Records(text, ranges);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "each", v => ((Records)v).Each());
      }

      public Value Each()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.LoopParameters();
               var records = State.RecordPattern.Split(text.Text);
               var subtexts = new List<string>();
               for (var i = 0; i < ranges.Length; i++)
               {
                  var range = ranges[i];
                  var startIndex = range.StartIndex;
                  var stopIndex = range.StopIndex;
                  var subtext = records.RangeOf(startIndex, stopIndex).Listify(State.RecordSeparator.Text);
                  assistant.SetLoopParameters(subtext, i);
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

                  subtexts.Add(result != null ? result.Text : subtext);
               }

               var index = 0;
               var newText = new List<string>();
               for (var i = 0; i < ranges.Length; i++)
               {
                  var range = ranges[i];
                  var startIndex = range.StartIndex;
                  var stopIndex = range.StopIndex;
                  while (index < startIndex)
                     newText.Add(records[index++]);

                  newText.Add(subtexts[i]);
                  index = stopIndex + 1;
               }
               for (var i = index; i < records.Length; i++)
                  newText.Add(records[i]);

               text.Text = newText.Listify(State.RecordSeparator.Text);
               return text;
            }

            return text;
         }
      }
   }
}