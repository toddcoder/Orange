using System.Collections.Generic;
using System.Linq;
using Orange.Library;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.Objects;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static System.Console;
using static System.Linq.Enumerable;
using static Standard.Types.Maybe.AttemptFunctions;

namespace Orange.REPL
{
   public class Code : IConsole
   {
      const string REGEX_INDENTS = "^ /s* ('if' | 'elseif' | 'else' | 'while' | 'for' | 'match' | 'case' | 'class' |" +
         "'static') /b";

      List<string> lines;
      int indentation;
      string indent;
      bool indenting;

      public Code()
      {
         lines = new List<string>();
         indentation = 0;
         indent = "";
         indenting = false;
      }

      public string Prompt => indentation == 0 ? "" : ".".Repeat(indentation);

      public void UndoLastLine()
      {
         if (lines.Count > 0)
            lines.RemoveAt(lines.Count - 1);
      }

      public void AddLine(string line)
      {
         if (advance(line) || regress(line))
            return;
         if (!indenting && line.IsMatch("^ ('print' | 'write' | 'put') ('ln')?"))
            return;

         var field = getField(line);

         lines.Add($"{indent}{line}");
         field.If(f => lines.Add($"{indent}{f}"));
         Execute().If(WriteLine).Else(e =>
         {
            WriteLine(e.Message);
            UndoLastLine();
         });
      }

      static IMaybe<string> getField(string line)
      {
         return line.Matches("^ ('va' ('l' | 'r') /s+)? /(/w+) /s* '='").Map(m => m.FirstGroup);
      }

      void doIndentation(int increment = 1)
      {
         indentation += increment;
         indent = "\t".Repeat(indentation);
         indenting = indentation > 0;
      }

      bool advance(string line)
      {
         if (line.IsMatch(REGEX_INDENTS) || lineMatchesFunction(line))
         {
            lines.Add($"{indent}{line}");
            doIndentation();
            return true;
         }

         return false;
      }

      static bool lineMatchesFunction(string line) => line.IsMatch("^ /s* 'func' /b .* ')' $");

      bool regress(string line)
      {
         if (indenting && line == "")
         {
            doIndentation(-1);
            return true;
         }

         return false;
      }

      public void Print(string text) => Write(text);

      public string Read() => ReadLine();

      public void Reset()
      {
         indentation = 0;
         indent = "";
         indenting = false;
         lines.Clear();
      }

      public IResult<string> Execute() => tryTo(() =>
      {
         var orange = new Library.Orange(ToString(), console: this);
         orange.Execute();
         return $"{orange.LastValue} | {orange.LastType}";
      });

      public override string ToString() => lines.Listify("\n");

      public string List()
      {
         var format = $"d.{lines.Count.ToString().Length}";
         return Range(0, lines.Count).Select(i => $"{i.FormatAs(format)} | {lines[i]}").Listify("\n");
      }

      public IResult<string> Edit(string line)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(line, "^ '#edit' /s+ /(/d+) /s+ /@"))
         {
            var index = matcher.FirstGroup.Int32();
            if (index.IsFailed)
               return new Failure<string>(index.Exception);

            var newLine = matcher.SecondGroup;

            var attempt = assert(index.Value.Between(0).Until(lines.Count), () =>
            {
               var tabs = lines[index.Value].Substitute("^ /(/t*) @", "$1");
               lines[index.Value] = $"{tabs}{newLine}";
               return Unit.Value;
            }, "Out of range index");
            if (attempt.IsFailed)
               return new Failure<string>(attempt.Exception);
         }

         return Execute();
      }

      public IResult<string> Delete(string line)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(line, "^ '#del' ('ete')? /s+ /(/d+)@"))
         {
            var index = matcher.FirstGroup.Int32();
            if (index.IsFailed)
               return new Failure<string>(index.Exception);

            var attempt = assert(index.Value.Between(0).Until(lines.Count), () =>
            {
               lines.RemoveAt(index.Value);
               return Unit.Value;
            }, "Out of range index");
            if (attempt.IsFailed)
               return new Failure<string>(attempt.Exception);
         }

         return Execute();
      }
   }
}