using System.IO;
using Orange.Library;

namespace OrangePlayground
{
   public class PlaygroundConsole : IConsole
   {
      TextWriter writer;
      TextReader reader;

      public PlaygroundConsole(TextWriter writer, TextReader reader)
      {
         this.writer = writer;
         this.reader = reader;
      }

      public void Print(string text) => writer.Write(text);

      public string Read() => reader.ReadLine();
   }
}