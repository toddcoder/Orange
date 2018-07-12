using Standard.Computer;

namespace OrangePlayground
{
   public class Settings
   {
      public string LastFile { get; set; } = "unknown.orange";

      public string DefaultFolder { get; set; } = FolderName.Current.FullPath;

      public string Text { get; set; } = "";

      public bool Manual { get; set; }

      public string EditorFont { get; set; } = "Consolas";

      public int EditorFontSize { get; set; } = 14;

      public string ConsoleFont { get; set; } = "Consolas";

      public int ConsoleFontSize { get; set; } = 12;

      public string ErrorFont { get; set; } = "Consolas";

      public int ErrorFontSize { get; set; } = 12;
   }
}