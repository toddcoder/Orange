using System.IO;
using Orange.Library.Managers;
using Standard.Computer;
using Standard.Types.Strings;
using static System.StringComparison;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class File : Value, INSGeneratorSource
   {
      public class FileLineGenerator : NSGenerator
      {
         File file;

         public FileLineGenerator(INSGeneratorSource generatorSource)
            : base(generatorSource) => file = (File)generatorSource;

         public override void Reset()
         {
            base.Reset();
            file.Reader = file.fileName.Reader();
         }

         public override Value Next()
         {
            var line = base.Next();
            if (line.IsNil)
               file.Reader?.Dispose();
            return line;
         }
      }

      FileName fileName;
      Block filter;

      public File(string fileName) => this.fileName = fileName;

      public File(FileName fileName) => this.fileName = fileName;

      public override int Compare(Value value) => string.Compare(fileName.ToString(), value.ToString(), Ordinal);

      public override string Text
      {
         get { return fileName.Text; }
         set { }
      }

      public override double Number
      {
         get { return Text.ToDouble(); }
         set { }
      }

      public override ValueType Type => ValueType.File;

      public override bool IsTrue => fileName.Length > 0;

      public override Value Clone() => new File(fileName);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "succ", v => ((File)v).Succ());
         manager.RegisterMessage(this, "pred", v => ((File)v).Pred());
         manager.RegisterMessage(this, "exists", v => ((File)v).Exists());
         manager.RegisterMessage(this, "len", v => ((File)v).Len());
         manager.RegisterMessage(this, "recs", v => ((File)v).Recs());
         manager.RegisterMessage(this, "lines", v => ((File)v).Lines());
         manager.RegisterMessage(this, "name", v => ((File)v).Name());
         manager.RegisterMessage(this, "nameExt", v => ((File)v).NameExtension());
         manager.RegisterMessage(this, "ext", v => ((File)v).Extension());
         manager.RegisterMessage(this, "array", v => ((File)v).ToArray());
      }

      public Value If()
      {
         filter = Arguments.Executable;
         return this;
      }

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
            return filter == null ? standardLoop(assistant) : filteredLoop(assistant);
      }

      Value standardLoop(ParameterAssistant assistant)
      {
         var block = assistant.Block();
         if (block == null)
            return this;

         assistant.LoopParameters();
         using (var fileNameReader = fileName.Reader())
         {
            string line;
            var index = 0;
            while ((line = fileNameReader.ReadLine()) != null)
            {
               assistant.SetLoopParameters(line, index++);
               block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }
            }
         }

         return this;
      }

      Value filteredLoop(ParameterAssistant assistant)
      {
         var block = assistant.Block();
         if (block == null)
            return this;

         assistant.LoopParameters();
         using (var fileNameReader = fileName.Reader())
         {
            string line;
            var index = 0;
            while ((line = fileNameReader.ReadLine()) != null)
            {
               assistant.SetLoopParameters(line, index++);
               if (filter.IsTrue)
                  block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }
            }
         }

         return this;
      }

      public override string ToString() => fileName.ToString();

      public StreamReader Reader { get; set; }

      public INSGenerator GetGenerator() => new FileLineGenerator(this);

      public Value Next(int index) => Reader?.ReadLine() ?? (Value)NilValue;

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);

      public Value Succ()
      {
         var name = fileName.Name;
         name = name.Succ();
         var newFileName = fileName.Folder.File(name, fileName.Extension);
         return new File(newFileName);
      }

      public Value Pred()
      {
         var name = fileName.Name;
         name = name.Pred();
         var newFileName = fileName.Folder.File(name, fileName.Extension);
         return new File(newFileName);
      }

      public Value Exists() => fileName.Exists();

      public Value Len() => fileName.Length;

      public Value Recs() => new Array(State.RecordPattern.Split(fileName.Text));

      public Value Lines() => new Array(fileName.Lines);

      public Value Name() => fileName.Name;

      public Value NameExtension() => fileName.NameExtension;

      public Value Extension() => fileName.Extension;
   }
}