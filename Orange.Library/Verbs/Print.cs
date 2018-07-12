using System.Text;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Print : Verb, IStatement
   {
      public enum PrintType
      {
         Print,
         Manifest,
         Put
      }

      Block expression;
      PrintType printType;
      bool endLine;
      string result;
      string typeName;

      public Print(Block expression, PrintType printType, bool endLine)
      {
         this.expression = expression;
         this.printType = printType;
         this.endLine = endLine;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var value = expression.Evaluate();
         typeName = value.Type.ToString();
         var text = "";
         switch (printType)
         {
            case PrintType.Print:
               text = ValueAsString(value);
               result = text;
               break;
            case PrintType.Manifest:
               text = ValueAsRep(value);
               result = text;
               break;
            case PrintType.Put:
               var generator = value.PossibleIndexGenerator();
               if (generator.IsSome)
               {
                  var iterator = new NSIterator(generator.Value);
                  var builder = new StringBuilder();
                  while (true)
                  {
                     var next = iterator.Next();
                     if (next.IsNil)
                        break;

                     text = ValueAsString(next);
                     State.ConsoleManager.Put(text);
                     if (builder.Length != 0)
                        builder.Append(" ");
                     builder.Append(text);
                  }

                  result = builder.ToString();
               }
               else
               {
                  text = ValueAsString(value);
                  result = text;
                  State.ConsoleManager.Put(text);
               }

               return null;
         }

         if (endLine)
            State.ConsoleManager.ConsolePrintln(text);
         else
            State.ConsoleManager.ConsolePrint(text);

         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public override string ToString()
      {
         switch (printType)
         {
            case PrintType.Print:
               return endLine ? $"println {expression}" : $"print {expression}";
            case PrintType.Manifest:
               return endLine ? $"manifln {expression}" : $"manif {expression}";
            case PrintType.Put:
               return $"put {expression}";
            default:
               return "";
         }
      }
   }
}