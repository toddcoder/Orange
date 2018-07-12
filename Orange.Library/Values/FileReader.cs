using System;
using System.IO;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class FileReader : Value, IDisposable
   {
      StreamReader reader;
      string[] readLine;
      int readIndex;
      bool closed;

      public FileReader(StreamReader reader)
      {
         this.reader = reader;
         readLine = null;
         readIndex = -1;
         closed = false;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            return reader.ReadToEnd();
         }
         set
         {
         }
      }

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.FileReader;

      public override bool IsTrue => !closed;

      public override Value Clone() => new FileReader(reader);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "rec", v => ((FileReader)v).Record());
         manager.RegisterMessage(this, "field", v => ((FileReader)v).Field());
         manager.RegisterMessage(this, "close", v => ((FileReader)v).Close());
         manager.RegisterMessage(this, "open?", v => ((FileReader)v).Open());
      }

      public Value Record()
      {
         if (closed)
            return new Nil();

         var line = reader.ReadLine();
         return line ?? (Value)new Nil();
      }

      public Value Field()
      {
         if (closed)
            return new Nil();

         if (readLine == null || readIndex >= readLine.Length)
         {
            var line = reader.ReadLine();
            if (line == null)
               return new Nil();
            readLine = Runtime.State.FieldPattern.Split(line);
            readIndex = -1;
         }

         return readLine[readIndex++];
      }

      public Value Close()
      {
         reader.Close();
         closed = true;
         return this;
      }

      public Value Open() => !closed;

      public override string ToString() => "reader";

      public void Dispose() => Close();
   }
}