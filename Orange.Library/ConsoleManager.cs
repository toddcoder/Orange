using static Orange.Library.Runtime;

namespace Orange.Library
{
   public class ConsoleManager
   {
      bool putting;
      bool startedPrinting;

      public ConsoleManager()
      {
         putting = false;
         startedPrinting = false;
      }

      public IConsole UIConsole
      {
         get;
         set;
      }

      public string LastOutput
      {
         get;
         set;
      } = "";

      public void Print(string text)
      {
         if (startedPrinting)
            UIConsole?.Print(State.RecordSeparator.Text);
         else
            startedPrinting = true;
         UIConsole?.Print(text);
         putting = false;
         LastOutput = text;
      }

      public void Put(string text)
      {
         if (putting)
            UIConsole?.Print(State.FieldSeparator.Text);
         UIConsole?.Print(text);
         putting = true;
         if (!startedPrinting)
            startedPrinting = true;
         LastOutput = text;
      }

      public void Write(string text)
      {
         UIConsole?.Print(text);
         if (!startedPrinting)
            startedPrinting = true;
         LastOutput = text;
      }

      public void ConsolePrint(string text) => UIConsole?.Print(text);

      public void ConsolePrintln(string text)
      {
         putting = false;
         ConsolePrint(text);
         ConsolePrint(State.RecordSeparator.Text);
      }
   }
}