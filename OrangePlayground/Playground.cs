using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Orange.Library;
using Orange.Library.Values;
using Standard.Computer;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Dates;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using Standard.WinForms.Consoles;
using Standard.WinForms.Documents;
using static System.Math;
using static System.Windows.Forms.Application;
using static System.Windows.Forms.Clipboard;
using static Newtonsoft.Json.JsonConvert;
using static Orange.Library.Runtime;
using static Standard.Types.Arrays.ArrayFunctions;
using static Standard.WinForms.Consoles.TextBoxConsole.ConsoleColorType;
using static Standard.WinForms.Documents.Document;
using static Standard.Types.Maybe.MaybeFunctions;

namespace OrangePlayground
{
   public partial class Playground : Form
   {
      const string MENU_FILE = "File";
      const string MENU_EDIT = "Edit";
      const string MENU_BUILD = "Build";
      const string MENU_TEXT = "Text";
      const string MENU_INSERT = "Insert";

      Document document;
      TextBoxConsole outputConsole;
      TextBoxConsole errorConsole;
      TextWriter errorWriter;
      TextWriter consoleWriter;
      TextReader textReader;
      bool locked;
      bool manual;
      Stopwatch stopwatch;
      InteractiveFileCache fileCache;
      Settings settings;
      Hash<int, string> results;
      int[] tabStops;
      Menus menus;

      public Playground() => InitializeComponent();

      public IMaybe<FileName> PassedFileName { get; set; } = none<FileName>();

      static IMaybe<FileName> getSettingsFile()
      {
         FolderName folder = @"C:\Enterprise\Configurations\Orange";
         var file = folder + "orange.json";
         if (!file.Exists())
         {
            folder = @"C:\Configurations";
            file = folder + "orange.json";
         }
         return when(file.Exists(), () => file);
      }

      static FolderName getSettingsFolder()
      {
         FolderName folder = @"C:\Enterprise\Configurations\Orange";
         if (folder.Exists())
            return folder;

         folder = @"C:\Configurations";
         if (folder.Exists())
            return folder;

         return FolderName.Current;
      }

      static Settings getSettings() => getSettingsFile().FlatMap(f => DeserializeObject<Settings>(f.Text), () => new Settings());

      void Playground_Load(object sender, EventArgs e)
      {
         results = new AutoHash<int, string>(k => "");
         settings = getSettings();

         errorConsole = new TextBoxConsole(this, textErrors, settings.ErrorFont, settings.ErrorFontSize, Cathode);
         errorWriter = errorConsole.Writer();
         try
         {
            try
            {
               tabStops = array(32, 64, 96, 128);
               textEditor.SelectionTabs = tabStops;
            }
            catch (Exception exception)
            {
               displayException(exception);
            }

            outputConsole = new TextBoxConsole(this, textConsole, settings.ConsoleFont, settings.ConsoleFontSize, Quick);
            consoleWriter = outputConsole.Writer();
            textReader = outputConsole.Reader();

            document = new Document(this, textEditor, ".orange", "Orange", settings.EditorFont, settings.EditorFontSize);
            document.StandardMenus();
            menus = document.Menus;
            menus.Menu(MENU_FILE, "Set Current Folder", (s, evt) => setCurrentFolder());

            menus.Menu(MENU_EDIT, "Duplicate", (s, evt) => duplicate(), "^D");
            menus.Menu(MENU_EDIT, "Indent", (s, evt) => indent(), "^I");
            menus.Menu(MENU_EDIT, "Unindent", (s, evt) => unindent(), "^%I");

            menus.Menu($"&{MENU_BUILD}");
            menus.Menu(MENU_BUILD, "Run", (s, evt) => run(), "F5");
            menus.Menu(MENU_BUILD, "Manual", (s, evt) =>
            {
               manual = !manual;
               ((ToolStripMenuItem)s).Checked = manual;
            }, "^F5");

            menus.Menu($"&{MENU_TEXT}");
            menus.Menu(MENU_TEXT, "Clipboard -> Text", (s, evt) =>
            {
               var text = ClipboardText();
               if (text.IsSome)
               {
                  var windowsText = GetWindowsText(text.Value);
                  if (textText.SelectionLength == 0)
                     textText.Text = windowsText;
                  else
                     textText.SelectedText = windowsText;
                  tabs.SelectedTab = tabText;
               }
            }, "^|V");
            menus.Menu(MENU_TEXT, "Clipboard -> Append to Text", (s, evt) =>
            {
               var text = ClipboardText();
               if (text.IsSome)
               {
                  var windowsText = GetWindowsText(text.Value);
                  textText.AppendText(windowsText);
                  tabs.SelectedTab = tabText;
               }
            });
            menus.Menu(MENU_TEXT, "Clipboard -> Text & Run", (s, evt) =>
            {
               var text = ClipboardText();
               if (text.IsSome)
                  textText.Text = GetWindowsText(text.Value);
               DoEvents();
               run();
               var selectedText = textConsole.SelectionLength == 0 ? textConsole.Text : textConsole.SelectedText;
               selectedText = GetWindowsText(selectedText);
               SetText(selectedText);
            }, "F6");
            menus.Menu(MENU_TEXT, "Text -> Clipboard", (s, evt) =>
            {
               var text = textConsole.SelectionLength == 0 ? textConsole.Text : textConsole.SelectedText;
               text = GetWindowsText(text);
               SetText(text);
            }, "^|C");
            menus.Menu(MENU_TEXT, "Save text to file", (s, evt) =>
            {
               var text = textConsole.SelectionLength == 0 ? textConsole.Text : textConsole.SelectedText;
               text = GetWindowsText(text);
               if (dialogSave.ShowDialog() == DialogResult.OK)
               {
                  FileName file = dialogSave.FileName;
                  try
                  {
                     file.Text = text;
                  }
                  catch (Exception exception)
                  {
                     displayException(exception);
                  }
               }
            });

            menus.Menu($"&{MENU_INSERT}");
            menus.Menu(MENU_INSERT, "println", (s, evt) => insertText("println ", 0, 0), "^P");
            menus.Menu(MENU_INSERT, "println interpolated", (s, evt) => insertText("println $\"\"", -1, 0), "^%P");
            menus.Menu(MENU_INSERT, "print", (s, evt) => insertText("print ", 0, 0));
            menus.Menu(MENU_INSERT, "manifln", (s, evt) => insertText("manifln ", 0, 0), "^M");
            menus.Menu(MENU_INSERT, "manif", (s, evt) => insertText("manif", 0, 0));
            menus.Menu(MENU_INSERT, "put", (s, evt) => insertText("put ", 0, 0));
            menus.Menu(MENU_INSERT, "peek", (s, evt) => surround("peek(", ")"), "^K");
            menus.MenuSeparator(MENU_INSERT);
            menus.Menu(MENU_INSERT, "if template", (s, evt) => insertTemplate("if true", "true"), "^|I");
            menus.Menu(MENU_INSERT, "while template", (s, evt) => insertTemplate("while true", "true"), "^|W");
            menus.Menu(MENU_INSERT, "for template", (s, evt) => insertTemplate("for i in ^0", "^0"), "^|F");
            menus.Menu(MENU_INSERT, "func template", (s, evt) => insertTemplate("func proc()", "proc"), "^|P");
            menus.Menu(MENU_INSERT, "func template", (s, evt) => insertTemplate("func proc()", "proc"), "^|P");

            menus.CreateMainMenu(this);

            menus.StandardContextEdit(document);

            textEditor.ReassignHandle();

            locked = false;
            manual = false;
            stopwatch = new Stopwatch();
            fileCache = new InteractiveFileCache();

            textEditor.Paint += (s, evt) => paintResults(evt);

            setManual(settings.Manual);
            if (settings.DefaultFolder != null)
               FolderName.Current = settings.DefaultFolder;
            textText.Text = settings.Text.FromBase64(Encoding.UTF8) ?? "";
            if (PassedFileName.IsSome)
               document.Open(PassedFileName.Value);
            else if (settings.LastFile != null && ((FileName)settings.LastFile).Exists())
               document.Open(settings.LastFile);
         }
         catch (Exception exception)
         {
            displayException(exception);
         }
      }

      void setManual(bool value)
      {
         manual = value;
         ((ToolStripMenuItem)menus[MENU_BUILD].DropDownItems[1]).Checked = manual;
      }

      void paintResults(PaintEventArgs e)
      {
         try
         {
            var firstVisibleChar = textEditor.GetCharIndexFromPosition(new Point(0, 0));
            var firstVisibleLine = textEditor.GetLineFromCharIndex(firstVisibleChar);
            var resultLines = Enumerable.Range(0, textEditor.Lines.Length).Select(l => "").ToArray();
            var tops = Enumerable.Range(0, textEditor.Lines.Length).Select(l => 0f).ToArray();
            foreach (var item in results)
            {
               var lineIndex = textEditor.GetLineFromCharIndex(item.Key);
               resultLines[lineIndex] = lineIndex >= firstVisibleLine ?
                  item.Value.VisibleWhitespace(false).Truncate(128) : "";
               tops[lineIndex] = textEditor.GetPositionFromCharIndex(item.Key).Y;
            }

            var font = textEditor.Font;

            for (var i = firstVisibleLine; i < textEditor.Lines.Length; i++)
            {
               var result = resultLines[i];
               var line = textEditor.Lines[i];
               var lineSize = e.Graphics.MeasureString(line, font);
               var top = tops[i];
               if (result.IsNotEmpty())
               {
                  var resultSize = e.Graphics.MeasureString(result, font);
                  var remainingWidth = textEditor.ClientRectangle.Width - lineSize.Width;
                  resultSize.Width = Min(remainingWidth, resultSize.Width);
                  var location = new PointF(e.Graphics.ClipBounds.Width - resultSize.Width, top);
                  var fillRectangle = new RectangleF(location.X, location.Y + 1, resultSize.Width, resultSize.Height - 2);
                  e.Graphics.FillRectangle(Brushes.LightGreen, fillRectangle);
                  using (var pen = new Pen(SystemColors.ButtonShadow, 1))
                     e.Graphics.DrawRectangle(pen, getRectangle(fillRectangle));
                  var format = new StringFormat(StringFormatFlags.LineLimit);
                  e.Graphics.DrawString(result, font, Brushes.Black, location, format);
               }
               var tabSize = getSize(e.Graphics.MeasureString("\t", font));
               if (line.Matches("^ /(/t1%4)").If(out var matcher))
               {
                  var x = 8;
                  var chindex = textEditor.GetFirstCharIndexFromLine(i);
                  var y = textEditor.GetPositionFromCharIndex(chindex).Y;
                  var height = y + tabSize.Height / 2;
                  var tabStopIndex = matcher.FirstGroup.Length - 1;
                  var width = tabStops[tabStopIndex];
                  using (var pen = new Pen(Color.LightGray, 3))
                  {
                     pen.CustomEndCap = new AdjustableArrowCap(3, 3, true);
                     e.Graphics.DrawLine(pen, x, height, x + width, height);
                  }
                  using (var pen = new Pen(Color.LightGray, 1))
                     for (var j = 0; j < tabStopIndex; j++)
                     {
                        var tabStop = tabStops[j];
                        var left = x + tabStop;
                        e.Graphics.DrawLine(pen, left, height - 8, left, height + 8);
                     }
               }
            }
         }
         catch { }
      }

      static Rectangle getRectangle(RectangleF rect) => new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);

      static Size getSize(SizeF size) => new Size((int)size.Width, (int)size.Height);

      void duplicate()
      {
         var dupLine = "";

         if (textEditor.SelectionLength == 0)
         {
            var currentLineIndex = textEditor.CurrentLineIndex();
            if (textEditor.AtEnd() && textEditor.Text.IsMatch("/n $"))
               currentLineIndex--;
            var lines = textEditor.Lines;
            if (currentLineIndex.Between(0).Until(lines.Length))
               dupLine = lines[currentLineIndex].Copy();
         }
         else
            dupLine = textEditor.SelectedText;

         textEditor.AppendAtEnd(dupLine, "/n");
      }

      static void setCurrentFolder()
      {
         using (var dialog = new FolderBrowserDialog())
         {
            dialog.SelectedPath = FolderName.Current.ToString();
            if (dialog.ShowDialog() == DialogResult.OK)
               FolderName.Current = dialog.SelectedPath;
         }
      }

      void run() => update(true, true);

      void update(bool execute, bool fromMenu)
      {
         if (locked || textEditor.TextLength == 0)
            return;

         locked = true;

         if (manual)
         {
            labelResult.Text = "running...";
            DoEvents();
         }
         else if (fromMenu)
            document.Save();

         try
         {
            textConsole.Clear();
            textErrors.Clear();

            results.Clear();

            var orange = new Orange.Library.Orange(textEditor.Text, new ColorParser(textEditor), fileCache,
                  new PlaygroundConsole(consoleWriter, textReader))
               { Text = textText.Text, ModuleFolders = array(@"C:\Enterprise\Modules", @"C:\Enterprise\Orange") };
            stopwatch.Reset();
            stopwatch.Start();
            if (execute)
               orange.Execute();
            else
               orange.ColorizeOnly();
            stopwatch.Stop();
            orange.Colorize();
            document.Clean();

            var lastValue = orange.LastValue;
            var lastType = orange.LastType;
            var resultText = lastValue.IsNotEmpty() ? $"{lastValue} | {lastType}" : "";
            results = Block.Results;
            var elapsed = stopwatch.Elapsed.ToLongString(true);
            labelResult.Text = resultText;
            labalElapsed.Text = elapsed;
            tabs.SelectedTab = tabConsole;
            textEditor.Select();
            textEditor.Refresh();
         }
         catch (Exception exception)
         {
            displayException(exception);
         }
         finally
         {
            locked = false;
         }
      }

      void displayException(Exception exception)
      {
         errorWriter.WriteLine(exception.Message);
         consoleWriter.WriteLine("Dumping print buffer:");
         consoleWriter.WriteLine(State?.PrintBuffer ?? "");
         tabs.SelectedTab = tabErrors;
         textEditor.Select();
      }

      void textEditor_TextChanged(object sender, EventArgs e)
      {
         if (document == null)
            return;

         update(!manual, false);
         document.Dirty();
      }

      void insertText(string text, int selectionOffset, int length = -1)
      {
         textEditor.SelectedText = text;
         textEditor.SelectionStart += selectionOffset;
         if (length > -1)
            textEditor.SelectionLength = length;
      }

      void surround(string before, string after)
      {
         var selected = textEditor.SelectedText;
         var replacement = $"{before}{selected}{after}";
         textEditor.SelectedText = replacement;
      }

      void insertDelimiterText(string delimiter, int selectionOffset, int length, int halfLength = -1)
      {
         if (textEditor.SelectionLength == 0)
            insertText(delimiter, selectionOffset, length);
         else
         {
            var selection = textEditor.SelectedText;
            if (halfLength == -1)
               halfLength = delimiter.Length / 2;
            textEditor.SelectedText = delimiter.Take(halfLength) + selection + delimiter.Skip(halfLength);
         }
      }

      string textAtInsert(int take, int skip = 0) => textEditor.Text.Skip(textEditor.SelectionStart + skip).Take(take);

      void setTextAtInsert(int take, int skip = 0, string text = "")
      {
         textEditor.Select(textEditor.SelectionStart + skip, take);
         textEditor.SelectedText = text;
         textEditor.Select(textEditor.SelectionStart + skip, 0);
      }

      void moveSelectionRelative(int amount = 1) => textEditor.SelectionStart += amount;

      void textEditor_KeyPress(object sender, KeyPressEventArgs e)
      {
         switch (e.KeyChar)
         {
            case '(':
               insertDelimiterText("()", -1, 0);
               e.Handled = true;
               break;
            case ')':
               if (textAtInsert(1) == ")")
               {
                  moveSelectionRelative();
                  e.Handled = true;
               }
               break;
            case '{':
               insertDelimiterText("{}", -1, 0);
               e.Handled = true;
               break;
            case '}':
               if (textAtInsert(1) == "}")
               {
                  moveSelectionRelative();
                  e.Handled = true;
               }
               break;
            case '[':
               insertDelimiterText("[]", -1, 0);
               e.Handled = true;
               break;
            case ']':
               if (textAtInsert(1) == "]")
               {
                  moveSelectionRelative();
                  e.Handled = true;
               }
               break;
            case '\'':
               if (textAtInsert(1) == "'")
               {
                  moveSelectionRelative();
                  e.Handled = true;
                  break;
               }

               insertDelimiterText("''", -1, 0);
               e.Handled = true;
               break;
            case '"':
               if (textAtInsert(1) == "\"")
               {
                  moveSelectionRelative();
                  e.Handled = true;
                  break;
               }

               insertDelimiterText("\"\"", -1, 0);
               e.Handled = true;
               break;
            case ',':
               if (textAtInsert(1) == ",")
               {
                  moveSelectionRelative();
                  e.Handled = true;
               }
               break;
            case ';':
               if (textAtInsert(1) == ";")
               {
                  if (textAtInsert(1, -1) == "\n")
                     insertPass();
                  else
                     moveSelectionRelative();
                  e.Handled = true;
               }
               break;
         }
      }

      void Playground_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (document.FileName.If(out var fileName))
            try
            {
               settings.LastFile = fileName;
               settings.DefaultFolder = FolderName.Current.FullPath;
               settings.Text = textText.Text.ToBase64(Encoding.UTF8);
               settings.Manual = manual;
               var file = getSettingsFolder() + "orange.json";
               file.Text = SerializeObject(settings);
            }
            catch (Exception exception)
            {
               MessageBox.Show(exception.Message);
            }
      }

      void textEditor_KeyUp(object sender, KeyEventArgs e)
      {
         switch (e.KeyCode)
         {
            case Keys.Escape:
               if (textAtInsert(1) == "'" && textAtInsert(1, -1) == "'")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
               }
               else if (textAtInsert(1) == "\"" && textAtInsert(1, -1) == "\"")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
               }
               else if (textAtInsert(1) == ")" && textAtInsert(1, -1) == "(")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
               }
               else if (textAtInsert(1) == "]" && textAtInsert(1, -1) == "[")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
               }
               else if (textAtInsert(1) == "}" && textAtInsert(1, -1) == "{")
               {
                  e.Handled = true;
                  setTextAtInsert(1);
               }
               break;
            case Keys.F12:
               insertPass();
               e.Handled = true;
               break;
            case Keys.Right:
               if (e.Alt)
               {
                  matchPass(true);
                  e.Handled = true;
               }
               break;
            case Keys.Left:
               if (e.Alt)
               {
                  matchPass(false);
                  e.Handled = true;
               }
               break;
            case Keys.F1:
               if (getWord().If(out var word) && findWord(word).If(out var found))
                  insertText(found.Skip(word.Length), 0);
               e.Handled = true;
               break;
         }
      }

      IMaybe<string> getWord()
      {
         var startIndex = textEditor.SelectionStart;
         var begin = textEditor.Text.Take(startIndex);
         return begin.Matches("/(/w+) $").Map(m => m.FirstGroup);
      }

      IMaybe<string> findWord(string begin)
      {
         var matches = textEditor.Text.Split("-/w+").Distinct().Where(word => word.StartsWith(begin));
         return matches.FirstOrNone();
      }

      void matchPass(bool first)
      {
         var matches = textEditor.Text.Matches("/b 'pass' /b");
         if (matches.If(out var matcher))
         {
            var passMatch = matcher.GetMatch(first ? 0 : matcher.MatchCount - 1);
            textEditor.Select(passMatch.Index, passMatch.Length);
         }
      }

      string getIndent() => textEditor.CurrentLine().Matches("^ /t*").FlatMap(m => m[0], () => "");

      void insertTemplate(string template, string replacement)
      {
         var indent = getIndent();
         var expandedTemplate = $"{indent}{template}\n{indent}\tpass";
         var index = expandedTemplate.IndexOf(replacement, StringComparison.Ordinal);
         if (index == -1)
            insertText(expandedTemplate, 0, 0);
         else
         {
            index = -(expandedTemplate.Length - index);
            insertText(expandedTemplate, index, replacement.Length);
         }
      }

      void insertPass()
      {
         var indent = getIndent();
         var text = $"\n{indent}\tpass";
         insertText(text, -text.Length, 0);
      }

      IMaybe<(string[], int)> linesFromSelection()
      {
         var startIndex = textEditor.GetLineFromCharIndex(textEditor.SelectionStart);
         var stopIndex = textEditor.GetLineFromCharIndex(textEditor.SelectionStart + textEditor.SelectionLength);
         var lines = textEditor.Lines;
         if (startIndex.Between(0).Until(lines.Length) && stopIndex.Between(0).Until(lines.Length))
            return (lines.RangeOf(startIndex, stopIndex), startIndex).Some();

         return none<(string[], int)>();
      }

      void indent()
      {
         try
         {
            var lines = textEditor.Lines;
            if (linesFromSelection().If(out var fromSelection))
            {
               (var selectedLines, var offset) = fromSelection;
               var saved = saveSelection();
               for (var i = 0; i < selectedLines.Length; i++)
                  textEditor.Lines[i + offset] = $"\t{selectedLines[i]}";

               textEditor.Lines = lines;
               restoreSelection(saved);
            }
         }
         catch (Exception exception)
         {
            errorWriter.WriteLine(exception.Message);
         }
      }

      void unindent()
      {
         try
         {
            var lines = textEditor.Lines;
            if (linesFromSelection().Assign(out var fromSelection))
            {
               (var selectedLines, var offset) = fromSelection;
               var saved = saveSelection();
               for (var i = 0; i < selectedLines.Length; i++)
                  if (selectedLines[i].Matches("^ /t /@").If(out var matcher))
                     lines[i + offset] = matcher.FirstGroup;

               textEditor.Lines = lines;
               restoreSelection(saved);
            }
         }
         catch (Exception exception)
         {
            errorWriter.WriteLine(exception.Message);
         }
      }

      (int, int) saveSelection() => (textEditor.SelectionStart, textEditor.SelectionLength);

      void restoreSelection((int start, int length) saved) => textEditor.Select(saved.start, saved.length);

      void textEditor_VScroll(object sender, EventArgs e)
      {
         try
         {
            /*            var currentLine = textEditor.CurrentLineIndex();
                        var position = textResults.GetFirstCharIndexFromLine(currentLine);
                        if (position < 0)
                           return;

                        textResults.Select(position, 0);*/
         }
         catch { }
      }

      void textResults_SelectionChanged(object sender, EventArgs e)
      {
         try
         {
            //labelResult.Text = textResults.CurrentLine();
         }
         catch { }
      }
   }
}