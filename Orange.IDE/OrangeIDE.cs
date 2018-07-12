using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Orange.Library;
using Orange.Library.Debugging;
using Standard.Computer;
using Standard.Types.Collections;
using Standard.Types.Strings;
using Standard.Types.Dates;
using Standard.Types.Enumerables;
using Standard.Types.RegularExpressions;
using Debugger = Orange.Library.Debugging.Debugger;

namespace Orange.IDE
{
   public partial class OrangeIDE : Form
   {
      const string REGEX_LAMBDA_BEGINNING = @"(\(([^)]+)\)|(" + Runtime.REGEX_VARIABLE + @"))\s*[-=]>$";

      Document document;
      Font normalFont;
      Font boldFont;
      Font fancyFont;
      //OrangeSettings settings;
      bool locked;
      bool manual;
      Stopwatch stopwatch;
      //string text;
      bool verbose;
      bool autoVariable;
      InteractiveFileCache fileCache;
      FileName textFile;
      Stack<BackupText> backupTexts;
      bool fancy;
      Hash<int, BreakpointSave> breakpointSaves;
      //BreakpointSave breakHighlight;
      Matcher matcher;
      bool modifying;

      public OrangeIDE()
      {
         InitializeComponent();
         richCode.AllowDrop = true;
         stopwatch = new Stopwatch();
         document = new Document(this, richCode)
         {
            OpenFileDialog = dialogOpen,
            SaveFileDialog = dialogSave
         };
         locked = false;
         manual = false;
         verbose = false;
         fileCache = new InteractiveFileCache();
         textFile = @"C:\Enterprise\Orange\Orange.txt"; //OrangeSettings.SettingsFolder + "Orange.txt";
         backupTexts = new Stack<BackupText>();
         fancy = false;
         breakpointSaves = new Hash<int, BreakpointSave>();
         //breakHighlight = null;
         matcher = new Matcher();
         modifying = false;
      }

      public FileName CodeFile { get; set; }

      void saveText()
      {
         textFile.Text = textText.Text;
      }

      void retrieveText()
      {
         textText.Text = textFile.Exists() ? textFile.Text : "";
      }

      void OrangeIDE_Load(object sender, EventArgs e)
      {
         normalFont = richConsole.Font;
         boldFont = new Font(normalFont, FontStyle.Bold);
         fancyFont = new Font("Gabriola", 16);

         try
         {
            /*				settings = OrangeSettings.SettingsObject;
                        if (CodeFile != null && CodeFile.Exists())
                        {
                           locked = true;
                           document.Open(CodeFile);
                           locked = false;
                        }
                        else if (settings.LastSourceFile != null && settings.LastSourceFile.Exists())
                        {
                           locked = true;
                           document.Open(settings.LastSourceFile);
                           locked = false;
                        }
                        if (settings.CurrentFolder != null)
                        {
                           FolderName.Current = settings.CurrentFolder;
                           displayMessage("Current folder is " + settings.CurrentFolder, false);
                        }*/
            retrieveText();
            displayMode();
         }
         catch (Exception exception)
         {
            displayMessage(exception.Message, true);
         }
      }

      void displayMessage(string message, bool error)
      {
         labelStatus.Text = message;
         if (error)
         {
            labelStatus.ForeColor = Color.White;
            labelStatus.BackColor = Color.Red;
         }
         else
         {
            labelStatus.ForeColor = SystemColors.ControlText;
            labelStatus.BackColor = SystemColors.Info;
         }
      }

      void displayMode()
      {
         var fileName = document.FileName;
         Text = (manual ? "Orange IDE (manual)" : "Orange IDE (instant)") + (fileName.IsNotEmpty() ? " - " + fileName : "") +
            (document.IsDirty ? " *" : "");
      }

      static string trailingSpaces(string text)
      {
         var lines = text.Split(@"\r\n");
         var matcher = new Matcher();
         for (var i = 0; i < lines.Length; i++)
         {
            if (lines[i].IsEmpty())
               continue;

            if (matcher.IsMatch(lines[i], @"([ \t]*)$"))
            {
               Slicer space = matcher[0, 1];
               for (var j = 0; j < space.Length; j++)
                  space[j, 1] = space[j, 1] == "\t" ? "¬" : "•";

               matcher[0, 1] = space.ToString();
            }

            lines[i] = matcher.ToString();
         }

         return lines.Listify("\r\n");
      }

      void update(bool execute)
      {
         if (locked)
            return;

         locked = true;

         if (richCode.TextLength == 0)
         {
            locked = false;
            return;
         }

         try
         {
            if (document.FileName.IsNotEmpty() && document.IsDirty)
               document.Save();

            if (manual)
            {
               labelValue.Text = "running...";
               Application.DoEvents();
            }
            stopwatch.Start();
            var orange = new Library.Orange(richCode.Text, new ColorParser(richCode), fileCache)
            {
               Text = textText.Text,
               Ask = ask
            };
            var result = execute ? orange.Execute() : orange.ColorizeOnly();
            stopwatch.Stop();
            //if (!document.IsDirty)
            orange.Colorize();
            document.Clean();
            displayMode();
            if (verbose)
               result += "\r\n" + orange.VerboseText;
            if (autoVariable)
            {
               result += "\r\n" + orange.DumpAll();
               result = trailingSpaces(result);
            }
            if (tabConsole.SelectedTab.Text == "Console")
            {
               fancy = menuLanguageFancy.Checked;
               richConsole.Text = result;
               richConsole.SelectAll();
               if (autoVariable)
               {
                  richConsole.SelectionColor = Color.White;
                  richConsole.SelectionBackColor = Color.Blue;
                  richConsole.SelectionFont = fancy ? fancyFont : boldFont;
               }
               else
               {
                  richConsole.SelectionColor = Color.Yellow;
                  richConsole.SelectionBackColor = Color.Blue;
                  richConsole.SelectionFont = fancy ? fancyFont : normalFont;
               }
               richConsole.Select(0, 0);
            }
            else
               webBrowser.DocumentText = result;

            displayMessage(stopwatch.Elapsed.ToLongString(true), false);
            var lastValue = orange.LastValue;
            var lastType = orange.LastType;
            labelValue.Text = lastValue.IsEmpty() ? "" : $"{lastValue} | {lastType}";
         }
         catch (Exception exception)
         {
            displayMessage($"{exception.GetType().Name}: {exception.Message}", true);
            if (Runtime.State != null)
               richConsole.Text = "Dumping print buffer:\r\n" + Runtime.State.PrintBuffer;
         }
         finally
         {
            locked = false;
         }
      }

      void OrangeIDE_FormClosing(object sender, FormClosingEventArgs e)
      {
         try
         {
            document.Close(e);
            /*				settings.CurrentFolder = FolderName.Current;
                        if (document.File != null)
                           settings.LastSourceFile = document.File;*/
            saveText();
            //OrangeSettings.SettingsObject = settings;
         }
         catch (Exception exception)
         {
            labelStatus.Text = exception.Message;
         }
      }

      public static string GetClipboardText() => Clipboard.ContainsText(TextDataFormat.Text) ?
         Clipboard.GetText(TextDataFormat.Text) : null;

      void menuFileNew_Click(object sender, EventArgs e)
      {
         document.New();
         displayMode();
         richConsole.Clear();
      }

      void menuFileOpen_Click(object sender, EventArgs e)
      {
         document.Open();
         displayMode();
         richConsole.Clear();
      }

      void menuFileSave_Click(object sender, EventArgs e)
      {
         document.Save();
         displayMode();
      }

      void menuFileSaveAs_Click(object sender, EventArgs e)
      {
         document.SaveAs();
         displayMode();
      }

      void menuEditUndo_Click(object sender, EventArgs e)
      {
         richCode.Undo();
      }

      void menuEditRedo_Click(object sender, EventArgs e)
      {
         richCode.Redo();
      }

      void menuEditCut_Click(object sender, EventArgs e)
      {
         richCode.Cut();
      }

      void menuEditCopy_Click(object sender, EventArgs e)
      {
         richCode.Copy();
      }

      void menuEditPaste_Click(object sender, EventArgs e)
      {
         richCode.Paste();
      }

      void menuEditSelectAll_Click(object sender, EventArgs e)
      {
         richCode.SelectAll();
      }

      void menuLanguageRefresh_Click(object sender, EventArgs e)
      {
         fileCache.Clear();
         update(true);
      }

      void menuLanguageManual_Click(object sender, EventArgs e)
      {
         manual = !manual;
         menuLanguageManual.Checked = manual;
      }

      void richCode_TextChanged(object sender, EventArgs e)
      {
         update(!manual);
         document.Dirty();
         displayMode();
      }

      void menuLanguageVerbose_Click(object sender, EventArgs e)
      {
         verbose = !verbose;
         menuLanguageVerbose.Checked = verbose;
         update(true);
         displayMode();
      }

      void menuToolsSetWorkingFolder_Click(object sender, EventArgs e)
      {
         dialogFolder.SelectedPath = FolderName.Current.ToString();

         if (dialogFolder.ShowDialog() != DialogResult.OK)
            return;

         FolderName.Current = dialogFolder.SelectedPath;
         displayMessage($"Current folder is {FolderName.Current}", false);
         //settings.CurrentFolder = FolderName.Current;
      }

      void insertText(string text, int selectionOffset, int length = -1)
      {
         richCode.SelectedText = text;
         richCode.SelectionStart += selectionOffset;
         richCode.SelectionLength = length;
         displayMode();
      }

      void menuToolsInsertQuotes_Click(object sender, EventArgs e)
      {
         insertDelimiterText("\"\"", -1, 0);
      }

      void menuToolsInsertPrint_Click(object sender, EventArgs e)
      {
         insertText("$out = ", 0, 0);
      }

      void menuLanguageAutoVariables_Click(object sender, EventArgs e)
      {
         autoVariable = !autoVariable;
         menuLanguageAutoVariables.Checked = autoVariable;
      }

      void menuCodePasteDoubleQuotedText_Click(object sender, EventArgs e)
      {
         var clipboardText = GetClipboardText();
         clipboardText = $"\"{clipboardText.Replace("\"", "`\"")}\"";
         if (richCode.SelectionLength == 0)
            richCode.Text = clipboardText;
         else
            richCode.SelectedText = clipboardText;
      }

      void menuCodePasteSingleQuotedText_Click(object sender, EventArgs e)
      {
         var clipboardText = GetClipboardText();
         clipboardText = $"'{clipboardText.Replace("'", "`'")}'";
         if (richCode.SelectionLength == 0)
            richCode.Text = clipboardText;
         else
            richCode.SelectedText = clipboardText;
      }

      void menuConsoleCopyToClipboard_Click(object sender, EventArgs e)
      {
         Clipboard.SetText(richConsole.SelectionLength == 0 ? richConsole.Text : richConsole.SelectedText);
      }

      private void menuConsoleCopyToEditor_Click(object sender, EventArgs e)
      {
         var consoleText = richCode.Text;
         consoleText = $"\"{consoleText.Replace("\"", "`\"")}\"";
         if (richCode.SelectionLength == 0)
            richCode.Text = consoleText;
         else
            richCode.SelectedText = consoleText;
      }

      void menuConsoleSaveToFile_Click(object sender, EventArgs e)
      {
         if (dialogSaveToFile.ShowDialog() != DialogResult.OK)
            return;

         FileName file = dialogSaveToFile.FileName;
         if (file.Exists())
            file.Delete();
         file.Text = richConsole.Lines.Listify("\r\n");
      }

      void richConsole_SelectionChanged(object sender, EventArgs e)
      {
         var length = richConsole.SelectionLength;

         if (length <= 0)
            return;

         var message = $"{length} char{(length == 1 ? "" : "s")} selected";
         displayMessage(message, false);
      }

      void menuToolsInsertInterpolationQuotes_Click(object sender, EventArgs e) => insertText("$out = %%", -1, 0);

      void menuToolsInsertPatternDelimiters_Click(object sender, EventArgs e) => insertDelimiterText("‹  ›", -2, 0);

      void insertDelimiterText(string delimiter, int selectionOffset, int length, int halfLength = -1)
      {
         if (richCode.SelectionLength == 0)
            insertText(delimiter, selectionOffset, length);
         else
         {
            var selection = richCode.SelectedText;
            if (halfLength == -1)
               halfLength = delimiter.Length / 2;
            richCode.SelectedText = delimiter.Substring(0, halfLength) + selection + delimiter.Substring(halfLength);
         }
      }

      void menuToolsGetFile_Click(object sender, EventArgs e)
      {
         dialogOpenText.InitialDirectory = FolderName.Current.ToString();
         dialogOpenText.FileName = "";

         if (dialogOpenText.ShowDialog() != DialogResult.OK)
            return;

         var insertedText = "\"" + dialogOpenText.FileName + "\"";
         insertText(insertedText, 0, 0);
      }

      static string ask()
      {
         using (var askDialog = new Ask())
         {
            //askDialog.Prompt = prompt;
            askDialog.ShowDialog();
            return askDialog.RequestedText;
         }
      }

      void menuTextCut_Click(object sender, EventArgs e)
      {
         if (textText.SelectionLength > 0)
            textText.Cut();
      }

      void menuTextCopy_Click(object sender, EventArgs e)
      {
         if (textText.SelectionLength > 0)
            textText.Copy();
      }

      void menuTextPaste_Click(object sender, EventArgs e)
      {
         textText.Paste();
      }

      void menuTextClear_Click(object sender, EventArgs e)
      {
         textText.Clear();
      }

      void menuTextOpen_Click(object sender, EventArgs e)
      {
         if (dialogOpenText.ShowDialog() != DialogResult.OK)
            return;

         try
         {
            FileName file = dialogOpenText.FileName;
            textText.Text = file.Text;
         }
         catch (Exception exception)
         {
            displayMessage(exception.Message, true);
         }
      }

      void menuTextSetText_Click(object sender, EventArgs e)
      {
         var clipboardText = Clipboard.GetText();
         var lines = clipboardText.Split(@"\r\n|\r|\n");
         textText.Text = lines.Listify("\r\n");
      }

      void menuTextGetText_Click(object sender, EventArgs e)
      {
         var lines = textText.Text.Split(@"\r\n|\r|\n");
         Clipboard.SetText(lines.Listify("\r\n"));
      }

      void menuEditCodeAsText_Click(object sender, EventArgs e)
      {
         var code = richCode.Text.Copy();
         var dollarText = textText.Text.Copy();
         var backupText = new BackupText
         {
            Code = code,
            Text = dollarText
         };
         backupTexts.Push(backupText);
         richCode.Text = "";
         textText.Text = code;
      }

      void menuEditSaveTextToCode_Click(object sender, EventArgs e)
      {
         if (backupTexts.Count == 0)
            return;

         richCode.Text = richConsole.Text;
         var backupText = backupTexts.Pop();
         textText.Text = backupText.Text;
      }

      void menuEditAbandonCodeChanges_Click(object sender, EventArgs e)
      {
         if (backupTexts.Count == 0)
            return;

         var backupText = backupTexts.Pop();
         richCode.Text = backupText.Code;
         textText.Text = backupText.Text;
      }

      void menuInsertInterpolationQuotes_Click(object sender, EventArgs e)
      {
         insertDelimiterText("%%", -1, 0);
      }

      void menuEditDuplicate_Click(object sender, EventArgs e)
      {
         var line = CurrentLine;
         var lines = richCode.Lines;
         if (line > 0 && line < lines.Length)
         {
            var text = lines[line - 1];
            richCode.SelectedText = text;
         }
      }

      int CurrentLine => richCode.GetLineFromCharIndex(richCode.SelectionStart);

      void menuToolsInsertRecs_Click(object sender, EventArgs e)
      {
         insertText("$recs = ", 0, 0);
      }

      void menuLanguageFancy_Click(object sender, EventArgs e)
      {
         fancy = !fancy;
         menuLanguageFancy.Checked = fancy;
      }

      void menuToolsInsertOutQ_Click(object sender, EventArgs e)
      {
         insertText("$out? = ().if", -4, 0);
      }

      void menuToolsInsertEmptyArrayLiteral_Click(object sender, EventArgs e)
      {
         insertText("@()", -1, 0);
      }

      void menuRefactorVariable_Click(object sender, EventArgs e)
      {
         var index = richCode.SelectionStart;
         var value = richCode.SelectedText;
         richCode.SelectedText = "var";
         var line = richCode.GetLineFromCharIndex(index);
         var lines = new List<string>(richCode.Lines);
         lines.Insert(line, "var = " + value + ";");
         richCode.Lines = lines.ToArray();
         richCode.SelectionStart = index;
      }

      void menuRefactorRename_Click(object sender, EventArgs e)
      {
         var index = richCode.SelectionStart;
         var text = richCode.Text;
         string variable;
         string replacement;
         if (matcher.IsMatch(text, "(" + Runtime.REGEX_VARIABLE + ")/(" + Runtime.REGEX_VARIABLE + ")"))
         {
            variable = matcher[0, 1];
            replacement = matcher[0, 2];
            matcher[0] = replacement;
         }
         else
            return;

         if (variable == null || replacement == null)
            return;

         var vLength = variable.Length;
         var rLength = replacement.Length;
         var offset = vLength - rLength;
         var totalOffset = offset - 1;
         variable = @"\b" + variable.Escape() + @"\b";
         matcher.Evaluate(text, variable);
         for (var i = 0; i < matcher.MatchCount; i++)
         {
            matcher[i] = replacement;
            totalOffset += offset;
         }

         richCode.Text = matcher.ToString();
         richCode.SelectionStart = index + totalOffset;
      }

      void menuToolsInsertPrintBlock_Click(object sender, EventArgs e)
      {
         insertText("${}", -1, 0);
      }

      void richCode_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.Control)
            switch (e.KeyCode)
            {
               case Keys.Home:
                  richCode.SelectionStart = 0;
                  richCode.SelectionLength = 0;
                  e.Handled = true;
                  break;
               case Keys.End:
                  richCode.SelectionStart = richCode.TextLength;
                  richCode.SelectionLength = 0;
                  e.Handled = true;
                  break;
            }
         /*			if (e.KeyCode == Keys.Escape)
                     setModifying();*/
         /*			if (e.KeyCode == Keys.Enter && richCode.SelectionLength == 0 && richCode.Text.Sub(richCode.SelectionStart - 2, 3) == "{\n}")
                  {
                     var line = richCode.Lines[CurrentLine];
                     string indent;
                     if (matcher.IsMatch(line, @"^(\s+)"))
                        indent = matcher[0, 1] + "\t";
                     else
                        indent = "\t";
                     richCode.SelectedText = indent + "\n";
                     richCode.SelectionStart--;
                     e.Handled = true;
                  }*/
      }

      void menuToolsInsertClassDelimiters_Click(object sender, EventArgs e)
      {
         insertDelimiterText("|{}", -1, 0, 2);
      }

      void menuToolsInsertBlock_Click(object sender, EventArgs e)
      {
         insertDelimiterText("{}", -1, 0);
      }

      void menuToolsInsertClosure_Click(object sender, EventArgs e)
      {
         insertDelimiterText("{|| }", -3, 0, 4);
      }

      void menuToolsInsertSingleQuotes_Click(object sender, EventArgs e)
      {
         insertDelimiterText("''", -1, 0);
      }

      void menuToolsInsertParentheses_Click(object sender, EventArgs e)
      {
         insertDelimiterText("()", -1, 0);
      }

      void menuToolsInsertOneLineClosure_Click(object sender, EventArgs e)
      {
         insertText("|| -> ;", -1, 0);
      }

      void menuToolsInsertOneLineClosureArguments_Click(object sender, EventArgs e)
      {
         insertText("|| -> ;", -6, 0);
      }

      string getSelectedText() => richCode.SelectionLength == 0 ? getSelectedLine() : richCode.SelectedText;

      void setSelectedText(string text)
      {
         if (richCode.SelectionLength == 0)
            setSelectedLine(text);
         else
            richCode.SelectedText = text;
      }

      string getSelectedLine()
      {
         var line = richCode.GetLineFromCharIndex(richCode.SelectionStart);
         var lines = richCode.Lines;
         return lines[line];
      }

      void setSelectedLine(string text)
      {
         var line = richCode.GetLineFromCharIndex(richCode.SelectionStart);
         var lines = richCode.Lines;
         lines[line] = text.TrimEnd();
         richCode.Lines = lines;
      }

      void menuToolsConvertToOneLineClosure_Click(object sender, EventArgs e)
      {
         var index = richCode.SelectionStart;
         var text = getSelectedText();
         if (matcher.IsMatch(text, @"{\|([^|]*)\|\s*"))
         {
            var arguments = matcher[0, 1];
            matcher[0] = $"|{arguments}| -> ";
            text = matcher.ToString();
            if (matcher.IsMatch(text, "}([^}]*)$"))
            {
               var rest = matcher[0, 1];
               matcher[0] = $";{rest}";
               text = matcher.ToString();
               setSelectedText(text);
               richCode.SelectionStart = index;
            }
         }
      }

      void menuToolsConvertToStandardClosure_Click(object sender, EventArgs e)
      {
         var index = richCode.SelectionStart;
         var text = getSelectedText();
         if (matcher.IsMatch(text, @"\|([^|]*)\|\s*->\s*([^;]*);"))
         {
            var arguments = matcher[0, 1];
            var code = matcher[0, 2];
            matcher[0] = $"{{|{arguments}| {code}}} ";
            text = matcher.ToString();
            setSelectedText(text);
            richCode.SelectionStart = index;
         }
      }

      void menuLanguageBuild_Click(object sender, EventArgs e)
      {
         /*			var buildFile = new FileName(document.File.Folder, document.File.Name, ".xorange");
                  var compiler = new OrangeCompiler(richCode.Text);
                  var block = compiler.Compile();
                  buildFile.Text = block.ToGraph("$root").ToString();*/
      }

      void richCode_KeyPress(object sender, KeyPressEventArgs e)
      {
         switch (e.KeyChar)
         {
            case '(':
               insertDelimiterText("()", -1, 0);
               e.Handled = true;
               break;
            case '{':
               //insertBrackets();
               insertDelimiterText("{}", -1, 0);
               e.Handled = true;
               break;
            case '[':
               insertDelimiterText("[]", -1, 0);
               e.Handled = true;
               break;
            case '\'':
               insertDelimiterText("''", -1, 0);
               e.Handled = true;
               break;
            case '"':
               insertDelimiterText("\"\"", -1, 0);
               e.Handled = true;
               break;
            /*				case ',':
                           insertText(", ", 0, 0);
                           e.Handled = true;
                           break;*/
         }
      }

      void insertBrackets()
      {
         if (richCode.TextLength == 0)
         {
            if (modifying)
            {
               richCode.Text = "{}";
               richCode.Select(1, 0);
               modifying = false;
            }
            else
            {
               richCode.Text = "{\r\t\r}";
               richCode.Select(3, 0);
            }
            return;
         }

         var lineNumber = CurrentLine;
         var lines = richCode.Lines;
         if (lineNumber < 0 || lineNumber >= lines.Length)
            return;

         var line = lines[lineNumber];
         var indent = line.Substitute(@"^(\s*).*$", "$1");
         var offset = -indent.Length - 2;
         if (richCode.SelectionLength == 0)
         {
            string replacement;
            if (matcher.IsMatch(line, REGEX_LAMBDA_BEGINNING))
            {
               var matched = matcher[0];
               var length = matched.Length;
               richCode.Select(richCode.SelectionStart - length, length);
               if (modifying)
               {
                  replacement = "{" + matched + "}";
                  modifying = false;
                  offset = -1;
               }
               else
                  replacement = "{" + matched + "\n" + indent + "\t\n" + indent + "}";
            }
            else if (modifying)
            {
               replacement = "{}";
               offset = -1;
               modifying = false;
            }
            else
               replacement = "{\r" + indent + "\t\n" + indent + "}";
            insertText(replacement, offset, 0);
         }
         else
         {
            var text = richCode.SelectedText;
            var replacement = "{\r" + indent + "\t" + text + "\n" + indent + "}";
            insertText(replacement, offset, 0);
         }
      }

      void menuDebugEnabled_Click(object sender, EventArgs e)
      {
         Debugger.DebuggingState = Debugger.IsDebugging ? null : new Debugger();
         menuDebugEnabled.Checked = menuDebugBreakpoint.Enabled = menuDebugStep.Enabled = menuDebugStepInto.Enabled = Debugger.IsDebugging;
         if (Debugger.IsDebugging)
         {
            Debugger.DebuggingState.Step += handleBreak;
            update(false);
         }
         else
            Debugger.DebuggingState.Step -= handleBreak;
      }

      void handleBreak(Position position)
      {
         //richCode.s
      }

      void menuDebugBreakpoint_Click(object sender, EventArgs e)
      {
         try
         {
            locked = true;
            var lineIndex = CurrentLine;
            Debugger.DebuggingState.ToggleBreakpoint(lineIndex);
            if (Debugger.DebuggingState.IsBreakpointSet(lineIndex))
            {
               var line = Debugger.DebuggingState.Line(lineIndex);
               var breakpointSave = new BreakpointSave(line.Position);
               breakpointSaves[lineIndex] = breakpointSave;
               breakpointSave.Begin(richCode);
               breakpointSave.Save(richCode);
               richCode.Select(line.Position.StartIndex, line.Position.Length);
               richCode.SelectionColor = Color.White;
               richCode.SelectionBackColor = Color.Red;
               breakpointSave.End(richCode);
            }
            else
            {
               var breakpointSave = breakpointSaves[lineIndex];
               breakpointSave.Begin(richCode);
               breakpointSave.Restore(richCode);
               breakpointSave.End(richCode);
            }
         }
         catch (Exception exception)
         {
            displayMessage(exception.Message, true);
         }
         finally
         {
            locked = false;
         }
      }

      void menuDebugStep_Click(object sender, EventArgs e)
      {
         if (Debugger.IsDebugging)
            Debugger.DebuggingState.DebugMode = Debugger.DebugModeType.Step;
         update(true);
      }

      void menuDebugStepInto_Click(object sender, EventArgs e)
      {
         if (Debugger.IsDebugging)
            Debugger.DebuggingState.DebugMode = Debugger.DebugModeType.StepInto;
         update(true);
      }

      void setModifying()
      {
         modifying = true;
         displayMessage("waiting for next key", false);
      }
   }
}