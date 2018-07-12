using System.Collections;
using System.Collections.Generic;
using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
   public class StringIterator : Value, IIterator, IEnumerable<Value>
   {
      string start;
      string stop;

      public StringIterator(string start, string stop)
      {
         this.start = start;
         this.stop = stop;
      }

      public override int Compare(Value value)
      {
         return 0;
      }

      public override string Text
      {
         get
         {
            return "";
         }
         set
         {
         }
      }

      public override double Number
      {
         get
         {
            return 0;
         }
         set
         {
         }
      }

      public override ValueType Type
      {
         get
         {
            return ValueType.Iterator;
         }
      }

      public override bool IsTrue
      {
         get
         {
            return false;
         }
      }

      public override Value Clone()
      {
         return new StringIterator(start, stop);
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "for", v => ((StringIterator)v).For());
         manager.RegisterMessage(this, "range", v => ((StringIterator)v).Range());
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((IntIterator)v).Apply());
      }

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.IteratorParameter();
               var count = 0;
               for (var current = start; current.CompareTo(stop) <= 0 && count++ < Runtime.MAX_LOOP; current = current.Succ())
               {
                  assistant.SetIteratorParameter(current);
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
            return this;
         }
      }

      public Value Range()
      {
         return new StringRange(start, stop)
         {
            Increment = (int)Increment.Number
         };
      }

      public Value Increment
      {
         get;
         set;
      }

      public void Iterate(Lambda lambda)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = lambda.Block;

            if (block == null)
               return;
            assistant.IteratorParameter(lambda.Parameters);
            var count = 0;
            for (var current = start; current.CompareTo(stop) <= 0 && count++ < Runtime.MAX_LOOP; current = current.Succ())
            {
               assistant.SetIteratorParameter(current);
               var text = block.Evaluate().Text;
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;
               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return;
               }
               Runtime.State.Print(text);
            }
         }
      }

      public IEnumerator<Value> GetEnumerator()
      {
         var count = 0;
         for (var current = start; current.CompareTo(stop) <= 0 && count++ < Runtime.MAX_LOOP; current = current.Succ())
            yield return current;
      }

      public override string ToString()
      {
         return string.Format("{0}|{1}", start.Quotify(@"`"""), stop.Quotify(@"`"""));
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public Value Apply()
      {
         var applyValue = Arguments.ApplyValue;
         var closure = applyValue as Lambda;
         if (closure != null)
            Iterate(closure);
         return this;
      }
   }
}