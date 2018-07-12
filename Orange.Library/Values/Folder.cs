using System.Linq;
using Orange.Library.Managers;
using Standard.Computer;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class Folder : Value, INSGeneratorSource
   {
      FolderName folderName;
      FileName[] fileNames;

      public Folder(string folderName)
      {
         this.folderName = folderName;
         fileNames = new FileName[0];
      }

      public Folder(FolderName folderName)
      {
         this.folderName = folderName;
         fileNames = new FileName[0];
      }

      public override int Compare(Value value) => string.Compare(folderName.ToString(), value.ToString(), System.StringComparison.Ordinal);

      public override string Text
      {
         get { return folderName.FullPath; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Folder;

      public override bool IsTrue => folderName.FileCount > 0;

      public override Value Clone() => new Folder(folderName);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "folders", v => ((Folder)v).Folders());
         manager.RegisterMessage(this, "files", v => ((Folder)v).Files());
      }

      public Value Folders() => new Array(folderName.Folders.Select(f => new Folder(f)));

      public Value Files() => ToArray();

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index)
      {
         if (index == 0)
            fileNames = folderName.Files.ToArray();
         if (index < fileNames.Length)
            return new File(fileNames[index]);
         return NilValue;
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => new Array(folderName.Files.Select(f => new File(f)));

      public override string ToString() => folderName.ToString();
   }
}