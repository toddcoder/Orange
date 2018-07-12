using System;
using Orange.Library.Managers;
using Standard.Types.Strings;
using System.IO;

namespace Orange.Library.Values
{
   public class FileLines : Value
   {
      string fileName;
      string filter;
      Lambda lambda;

      public FileLines(string fileName)
      {
         this.fileName = fileName;
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

      public override ValueType Type => ValueType.FLines;

      public override bool IsTrue => false;

      public override Value Clone() => new FileLines(fileName);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "for", v => ((FileLines)v).For());
         manager.RegisterMessage(this, "if", v => ((FileLines)v).If());
         manager.RegisterProperty(this, "filter", v => ((FileLines)v).GetFilter(),
            v => ((FileLines)v).SetFilter());
      }

      public Value If()
      {
         lambda = new Lambda(RegionManager.Regions.Current, Arguments.Executable, Arguments.Parameters, false);
         return this;
      }

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.LoopParameters();
               Func<string, bool> func = l => true;
               if (filter.IsNotEmpty())
                  func = l => l.IndexOf(filter, StringComparison.Ordinal) > -1;
               else if (lambda != null)
                  func = l => lambda.Evaluate(new Arguments(l)).IsTrue;
               using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
               using (var reader = new StreamReader(stream))
               {
                  string line;
                  var index = 0;
                  while ((line = reader.ReadLine()) != null)
                     if (func(line))
                     {
                        assistant.SetLoopParameters(line, index++);
                        block.Evaluate();
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