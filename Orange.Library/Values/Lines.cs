using System;
using System.IO;
using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class Lines : Value
   {
      string text;
      string filter;

      public Lines(string text)
      {
         this.text = text;
         filter = "";
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get;
         set;
      }

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.Lines;

      public override bool IsTrue => false;

      public override Value Clone() => new Lines(text);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "for", v => ((Lines)v).For());
         manager.RegisterProperty(this, "filter", v => ((Lines)v).GetFilter(), v => ((Lines)v).SetFilter());
      }

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.LoopParameters();
               if (filter.IsEmpty())
                  using (var reader = new StringReader(text))
                  {
                     string line;
                     var index = 0;
                     while ((line = reader.ReadLine()) != null)
                     {
                        assistant.SetLoopParameters(line, index++);
                        block.Evaluate();
                        var signal = ParameterAssistant.Signal();
                        if (signal == ParameterAssistant.SignalType.Breaking)
                           break;
                        switch (signal)
                        {
                           case ParameterAssistant.SignalType.ReturningNull:
                              return null;
                           case ParameterAssistant.SignalType.Continuing:
                              continue;
                        }
                     }
                  }
               else
                  using (var reader = new StringReader(text))
                  {
                     string line;
                     var index = 0;
                     while ((line = reader.ReadLine()) != null)
                        if (line.IndexOf(filter, StringComparison.Ordinal) > -1)
                        {
                           assistant.SetLoopParameters(line, index++);
                           block.Evaluate();
                           var signal = ParameterAssistant.Signal();
                           if (signal == ParameterAssistant.SignalType.Breaking)
                              break;
                           switch (signal)
                           {
                              case ParameterAssistant.SignalType.ReturningNull:
                                 return null;
                              case ParameterAssistant.SignalType.Continuing:
                                 continue;
                           }
                        }
                  }
               return null;
            }
            return null;
         }
      }

      public Value Filter() => new ValueAttributeVariable("filter", this);

      public Value GetFilter() => filter;

      public Value SetFilter()
      {
         filter = Arguments[0].Text;
         return filter;
      }
   }
}