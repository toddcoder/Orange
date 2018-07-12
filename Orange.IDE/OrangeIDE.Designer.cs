namespace Orange.IDE
{
   partial class OrangeIDE
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         var resources = new System.ComponentModel.ComponentResourceManager(typeof(OrangeIDE));
         this.menu = new System.Windows.Forms.MenuStrip();
         this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
         this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
         this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
         this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
         this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.menuEditCut = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
         this.menuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.menuEditCodeAsText = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEditSaveTextToCode = new System.Windows.Forms.ToolStripMenuItem();
         this.menuEditAbandonCodeChanges = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
         this.menuEditDuplicate = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
         this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsSetWorkingFolder = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertQuotes = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertPrint = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertInterpolationQuotes = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertPatternDelimiters = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsGetFile = new System.Windows.Forms.ToolStripMenuItem();
         this.menuInsertInterpolationQuotes = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertRecs = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertOutQ = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertEmptyArrayLiteral = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertPrintBlock = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertClassDelimiters = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertBlock = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertClosure = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertSingleQuotes = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertParentheses = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertOneLineClosure = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsInsertOneLineClosureArguments = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsConvertToOneLineClosure = new System.Windows.Forms.ToolStripMenuItem();
         this.menuToolsConvertToStandardClosure = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguage = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguageRefresh = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguageManual = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguageVerbose = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguageAutoVariables = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguageFancy = new System.Windows.Forms.ToolStripMenuItem();
         this.menuLanguageBuild = new System.Windows.Forms.ToolStripMenuItem();
         this.menuRefactor = new System.Windows.Forms.ToolStripMenuItem();
         this.menuRefactorVariable = new System.Windows.Forms.ToolStripMenuItem();
         this.menuRefactorRename = new System.Windows.Forms.ToolStripMenuItem();
         this.menuDebug = new System.Windows.Forms.ToolStripMenuItem();
         this.menuDebugEnabled = new System.Windows.Forms.ToolStripMenuItem();
         this.menuDebugBreakpoint = new System.Windows.Forms.ToolStripMenuItem();
         this.menuDebugStep = new System.Windows.Forms.ToolStripMenuItem();
         this.menuDebugStepInto = new System.Windows.Forms.ToolStripMenuItem();
         this.table = new System.Windows.Forms.TableLayoutPanel();
         this.richCode = new System.Windows.Forms.RichTextBox();
         this.menuCode = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.menuCodePasteDoubleQuotedText = new System.Windows.Forms.ToolStripMenuItem();
         this.menuCodePasteSingleQuotedText = new System.Windows.Forms.ToolStripMenuItem();
         this.status = new System.Windows.Forms.StatusStrip();
         this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
         this.tabConsole = new System.Windows.Forms.TabControl();
         this.tabPage1 = new System.Windows.Forms.TabPage();
         this.richConsole = new System.Windows.Forms.RichTextBox();
         this.menuConsole = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.menuConsoleCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
         this.menuConsoleCopyToEditor = new System.Windows.Forms.ToolStripMenuItem();
         this.menuConsoleSaveToFile = new System.Windows.Forms.ToolStripMenuItem();
         this.tabPage3 = new System.Windows.Forms.TabPage();
         this.textText = new System.Windows.Forms.TextBox();
         this.menuText = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.menuTextCut = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTextCopy = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTextPaste = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTextClear = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTextOpen = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTextSetText = new System.Windows.Forms.ToolStripMenuItem();
         this.menuTextGetText = new System.Windows.Forms.ToolStripMenuItem();
         this.tabPage2 = new System.Windows.Forms.TabPage();
         this.webBrowser = new System.Windows.Forms.WebBrowser();
         this.labelValue = new System.Windows.Forms.Label();
         this.dialogOpen = new System.Windows.Forms.OpenFileDialog();
         this.dialogSave = new System.Windows.Forms.SaveFileDialog();
         this.dialogFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.dialogSaveToFile = new System.Windows.Forms.SaveFileDialog();
         this.dialogOpenText = new System.Windows.Forms.OpenFileDialog();
         this.menu.SuspendLayout();
         this.table.SuspendLayout();
         this.menuCode.SuspendLayout();
         this.status.SuspendLayout();
         this.tabConsole.SuspendLayout();
         this.tabPage1.SuspendLayout();
         this.menuConsole.SuspendLayout();
         this.tabPage3.SuspendLayout();
         this.menuText.SuspendLayout();
         this.tabPage2.SuspendLayout();
         this.SuspendLayout();
         // 
         // menu
         // 
         this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuTools,
            this.menuLanguage,
            this.menuRefactor,
            this.menuDebug});
         this.menu.Location = new System.Drawing.Point(0, 0);
         this.menu.Name = "menu";
         this.menu.Size = new System.Drawing.Size(671, 24);
         this.menu.TabIndex = 0;
         this.menu.Text = "menuStrip1";
         // 
         // menuFile
         // 
         this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.toolStripSeparator,
            this.menuFileSave,
            this.menuFileSaveAs});
         this.menuFile.Name = "menuFile";
         this.menuFile.Size = new System.Drawing.Size(37, 20);
         this.menuFile.Text = "&File";
         // 
         // menuFileNew
         // 
         this.menuFileNew.Image = ((System.Drawing.Image)(resources.GetObject("menuFileNew.Image")));
         this.menuFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.menuFileNew.Name = "menuFileNew";
         this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
         this.menuFileNew.Size = new System.Drawing.Size(146, 22);
         this.menuFileNew.Text = "&New";
         this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
         // 
         // menuFileOpen
         // 
         this.menuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuFileOpen.Image")));
         this.menuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.menuFileOpen.Name = "menuFileOpen";
         this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
         this.menuFileOpen.Size = new System.Drawing.Size(146, 22);
         this.menuFileOpen.Text = "&Open";
         this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
         // 
         // toolStripSeparator
         // 
         this.toolStripSeparator.Name = "toolStripSeparator";
         this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
         // 
         // menuFileSave
         // 
         this.menuFileSave.Image = ((System.Drawing.Image)(resources.GetObject("menuFileSave.Image")));
         this.menuFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.menuFileSave.Name = "menuFileSave";
         this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
         this.menuFileSave.Size = new System.Drawing.Size(146, 22);
         this.menuFileSave.Text = "&Save";
         this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
         // 
         // menuFileSaveAs
         // 
         this.menuFileSaveAs.Name = "menuFileSaveAs";
         this.menuFileSaveAs.Size = new System.Drawing.Size(146, 22);
         this.menuFileSaveAs.Text = "Save &As";
         this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
         // 
         // menuEdit
         // 
         this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditUndo,
            this.menuEditRedo,
            this.toolStripSeparator3,
            this.menuEditCut,
            this.menuEditCopy,
            this.menuEditPaste,
            this.toolStripSeparator4,
            this.menuEditSelectAll,
            this.toolStripSeparator2,
            this.menuEditCodeAsText,
            this.menuEditSaveTextToCode,
            this.menuEditAbandonCodeChanges,
            this.toolStripMenuItem1,
            this.menuEditDuplicate});
         this.menuEdit.Name = "menuEdit";
         this.menuEdit.Size = new System.Drawing.Size(39, 20);
         this.menuEdit.Text = "&Edit";
         // 
         // menuEditUndo
         // 
         this.menuEditUndo.Name = "menuEditUndo";
         this.menuEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
         this.menuEditUndo.Size = new System.Drawing.Size(286, 22);
         this.menuEditUndo.Text = "&Undo";
         this.menuEditUndo.Click += new System.EventHandler(this.menuEditUndo_Click);
         // 
         // menuEditRedo
         // 
         this.menuEditRedo.Name = "menuEditRedo";
         this.menuEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
         this.menuEditRedo.Size = new System.Drawing.Size(286, 22);
         this.menuEditRedo.Text = "&Redo";
         this.menuEditRedo.Click += new System.EventHandler(this.menuEditRedo_Click);
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(283, 6);
         // 
         // menuEditCut
         // 
         this.menuEditCut.Image = ((System.Drawing.Image)(resources.GetObject("menuEditCut.Image")));
         this.menuEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.menuEditCut.Name = "menuEditCut";
         this.menuEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
         this.menuEditCut.Size = new System.Drawing.Size(286, 22);
         this.menuEditCut.Text = "Cu&t";
         this.menuEditCut.Click += new System.EventHandler(this.menuEditCut_Click);
         // 
         // menuEditCopy
         // 
         this.menuEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("menuEditCopy.Image")));
         this.menuEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.menuEditCopy.Name = "menuEditCopy";
         this.menuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
         this.menuEditCopy.Size = new System.Drawing.Size(286, 22);
         this.menuEditCopy.Text = "&Copy";
         this.menuEditCopy.Click += new System.EventHandler(this.menuEditCopy_Click);
         // 
         // menuEditPaste
         // 
         this.menuEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("menuEditPaste.Image")));
         this.menuEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.menuEditPaste.Name = "menuEditPaste";
         this.menuEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
         this.menuEditPaste.Size = new System.Drawing.Size(286, 22);
         this.menuEditPaste.Text = "&Paste";
         this.menuEditPaste.Click += new System.EventHandler(this.menuEditPaste_Click);
         // 
         // toolStripSeparator4
         // 
         this.toolStripSeparator4.Name = "toolStripSeparator4";
         this.toolStripSeparator4.Size = new System.Drawing.Size(283, 6);
         // 
         // menuEditSelectAll
         // 
         this.menuEditSelectAll.Name = "menuEditSelectAll";
         this.menuEditSelectAll.Size = new System.Drawing.Size(286, 22);
         this.menuEditSelectAll.Text = "Select &All";
         this.menuEditSelectAll.Click += new System.EventHandler(this.menuEditSelectAll_Click);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(283, 6);
         // 
         // menuEditCodeAsText
         // 
         this.menuEditCodeAsText.Name = "menuEditCodeAsText";
         this.menuEditCodeAsText.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
         this.menuEditCodeAsText.Size = new System.Drawing.Size(286, 22);
         this.menuEditCodeAsText.Text = "Code as $";
         this.menuEditCodeAsText.Click += new System.EventHandler(this.menuEditCodeAsText_Click);
         // 
         // menuEditSaveTextToCode
         // 
         this.menuEditSaveTextToCode.Name = "menuEditSaveTextToCode";
         this.menuEditSaveTextToCode.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End)));
         this.menuEditSaveTextToCode.Size = new System.Drawing.Size(286, 22);
         this.menuEditSaveTextToCode.Text = "Save $ to Code";
         this.menuEditSaveTextToCode.Click += new System.EventHandler(this.menuEditSaveTextToCode_Click);
         // 
         // menuEditAbandonCodeChanges
         // 
         this.menuEditAbandonCodeChanges.Name = "menuEditAbandonCodeChanges";
         this.menuEditAbandonCodeChanges.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemtilde)));
         this.menuEditAbandonCodeChanges.Size = new System.Drawing.Size(286, 22);
         this.menuEditAbandonCodeChanges.Text = "Abandon Code Changes";
         this.menuEditAbandonCodeChanges.Click += new System.EventHandler(this.menuEditAbandonCodeChanges_Click);
         // 
         // toolStripMenuItem1
         // 
         this.toolStripMenuItem1.Name = "toolStripMenuItem1";
         this.toolStripMenuItem1.Size = new System.Drawing.Size(283, 6);
         // 
         // menuEditDuplicate
         // 
         this.menuEditDuplicate.Name = "menuEditDuplicate";
         this.menuEditDuplicate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
         this.menuEditDuplicate.Size = new System.Drawing.Size(286, 22);
         this.menuEditDuplicate.Text = "D&uplicate";
         this.menuEditDuplicate.Click += new System.EventHandler(this.menuEditDuplicate_Click);
         // 
         // menuTools
         // 
         this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.menuToolsSetWorkingFolder,
            this.menuToolsInsertQuotes,
            this.menuToolsInsertPrint,
            this.menuToolsInsertInterpolationQuotes,
            this.menuToolsInsertPatternDelimiters,
            this.menuToolsGetFile,
            this.menuInsertInterpolationQuotes,
            this.menuToolsInsertRecs,
            this.menuToolsInsertOutQ,
            this.menuToolsInsertEmptyArrayLiteral,
            this.menuToolsInsertPrintBlock,
            this.menuToolsInsertClassDelimiters,
            this.menuToolsInsertBlock,
            this.menuToolsInsertClosure,
            this.menuToolsInsertSingleQuotes,
            this.menuToolsInsertParentheses,
            this.menuToolsInsertOneLineClosure,
            this.menuToolsInsertOneLineClosureArguments,
            this.menuToolsConvertToOneLineClosure,
            this.menuToolsConvertToStandardClosure});
         this.menuTools.Name = "menuTools";
         this.menuTools.Size = new System.Drawing.Size(48, 20);
         this.menuTools.Text = "&Tools";
         // 
         // customizeToolStripMenuItem
         // 
         this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
         this.customizeToolStripMenuItem.Size = new System.Drawing.Size(333, 22);
         this.customizeToolStripMenuItem.Text = "&Customize";
         // 
         // optionsToolStripMenuItem
         // 
         this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
         this.optionsToolStripMenuItem.Size = new System.Drawing.Size(333, 22);
         this.optionsToolStripMenuItem.Text = "&Options";
         // 
         // menuToolsSetWorkingFolder
         // 
         this.menuToolsSetWorkingFolder.Name = "menuToolsSetWorkingFolder";
         this.menuToolsSetWorkingFolder.Size = new System.Drawing.Size(333, 22);
         this.menuToolsSetWorkingFolder.Text = "&Set Working Folder";
         this.menuToolsSetWorkingFolder.Click += new System.EventHandler(this.menuToolsSetWorkingFolder_Click);
         // 
         // menuToolsInsertQuotes
         // 
         this.menuToolsInsertQuotes.Name = "menuToolsInsertQuotes";
         this.menuToolsInsertQuotes.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.Q)));
         this.menuToolsInsertQuotes.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertQuotes.Text = "Insert &Quotes";
         this.menuToolsInsertQuotes.Click += new System.EventHandler(this.menuToolsInsertQuotes_Click);
         // 
         // menuToolsInsertPrint
         // 
         this.menuToolsInsertPrint.Name = "menuToolsInsertPrint";
         this.menuToolsInsertPrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
         this.menuToolsInsertPrint.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertPrint.Text = "Insert &Print";
         this.menuToolsInsertPrint.Click += new System.EventHandler(this.menuToolsInsertPrint_Click);
         // 
         // menuToolsInsertInterpolationQuotes
         // 
         this.menuToolsInsertInterpolationQuotes.Name = "menuToolsInsertInterpolationQuotes";
         this.menuToolsInsertInterpolationQuotes.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.P)));
         this.menuToolsInsertInterpolationQuotes.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertInterpolationQuotes.Text = "Print Interpolation Quotes";
         this.menuToolsInsertInterpolationQuotes.Click += new System.EventHandler(this.menuToolsInsertInterpolationQuotes_Click);
         // 
         // menuToolsInsertPatternDelimiters
         // 
         this.menuToolsInsertPatternDelimiters.Name = "menuToolsInsertPatternDelimiters";
         this.menuToolsInsertPatternDelimiters.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemOpenBrackets)));
         this.menuToolsInsertPatternDelimiters.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertPatternDelimiters.Text = "Insert Pattern Delimiters";
         this.menuToolsInsertPatternDelimiters.Click += new System.EventHandler(this.menuToolsInsertPatternDelimiters_Click);
         // 
         // menuToolsGetFile
         // 
         this.menuToolsGetFile.Name = "menuToolsGetFile";
         this.menuToolsGetFile.ShortcutKeys = System.Windows.Forms.Keys.F11;
         this.menuToolsGetFile.Size = new System.Drawing.Size(333, 22);
         this.menuToolsGetFile.Text = "&Get File";
         this.menuToolsGetFile.Click += new System.EventHandler(this.menuToolsGetFile_Click);
         // 
         // menuInsertInterpolationQuotes
         // 
         this.menuInsertInterpolationQuotes.Name = "menuInsertInterpolationQuotes";
         this.menuInsertInterpolationQuotes.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
         this.menuInsertInterpolationQuotes.Size = new System.Drawing.Size(333, 22);
         this.menuInsertInterpolationQuotes.Text = "Insert Interpolation Quotes";
         this.menuInsertInterpolationQuotes.Click += new System.EventHandler(this.menuInsertInterpolationQuotes_Click);
         // 
         // menuToolsInsertRecs
         // 
         this.menuToolsInsertRecs.Name = "menuToolsInsertRecs";
         this.menuToolsInsertRecs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
         this.menuToolsInsertRecs.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertRecs.Text = "Insert $&recs";
         this.menuToolsInsertRecs.Click += new System.EventHandler(this.menuToolsInsertRecs_Click);
         // 
         // menuToolsInsertOutQ
         // 
         this.menuToolsInsertOutQ.Name = "menuToolsInsertOutQ";
         this.menuToolsInsertOutQ.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemQuestion)));
         this.menuToolsInsertOutQ.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertOutQ.Text = "Insert $out?";
         this.menuToolsInsertOutQ.Click += new System.EventHandler(this.menuToolsInsertOutQ_Click);
         // 
         // menuToolsInsertEmptyArrayLiteral
         // 
         this.menuToolsInsertEmptyArrayLiteral.Name = "menuToolsInsertEmptyArrayLiteral";
         this.menuToolsInsertEmptyArrayLiteral.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
         this.menuToolsInsertEmptyArrayLiteral.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertEmptyArrayLiteral.Text = "Insert empty array literal";
         this.menuToolsInsertEmptyArrayLiteral.Click += new System.EventHandler(this.menuToolsInsertEmptyArrayLiteral_Click);
         // 
         // menuToolsInsertPrintBlock
         // 
         this.menuToolsInsertPrintBlock.Name = "menuToolsInsertPrintBlock";
         this.menuToolsInsertPrintBlock.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
         this.menuToolsInsertPrintBlock.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertPrintBlock.Text = "Insert print block";
         this.menuToolsInsertPrintBlock.Click += new System.EventHandler(this.menuToolsInsertPrintBlock_Click);
         // 
         // menuToolsInsertClassDelimiters
         // 
         this.menuToolsInsertClassDelimiters.Name = "menuToolsInsertClassDelimiters";
         this.menuToolsInsertClassDelimiters.ShortcutKeys = System.Windows.Forms.Keys.F2;
         this.menuToolsInsertClassDelimiters.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertClassDelimiters.Text = "Insert Class Delimiters";
         this.menuToolsInsertClassDelimiters.Click += new System.EventHandler(this.menuToolsInsertClassDelimiters_Click);
         // 
         // menuToolsInsertBlock
         // 
         this.menuToolsInsertBlock.Name = "menuToolsInsertBlock";
         this.menuToolsInsertBlock.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.OemOpenBrackets)));
         this.menuToolsInsertBlock.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertBlock.Text = "Insert Block";
         this.menuToolsInsertBlock.Click += new System.EventHandler(this.menuToolsInsertBlock_Click);
         // 
         // menuToolsInsertClosure
         // 
         this.menuToolsInsertClosure.Name = "menuToolsInsertClosure";
         this.menuToolsInsertClosure.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.OemOpenBrackets)));
         this.menuToolsInsertClosure.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertClosure.Text = "Insert Closure";
         this.menuToolsInsertClosure.Click += new System.EventHandler(this.menuToolsInsertClosure_Click);
         // 
         // menuToolsInsertSingleQuotes
         // 
         this.menuToolsInsertSingleQuotes.Name = "menuToolsInsertSingleQuotes";
         this.menuToolsInsertSingleQuotes.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
         this.menuToolsInsertSingleQuotes.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertSingleQuotes.Text = "Insert single quotes";
         this.menuToolsInsertSingleQuotes.Click += new System.EventHandler(this.menuToolsInsertSingleQuotes_Click);
         // 
         // menuToolsInsertParentheses
         // 
         this.menuToolsInsertParentheses.Name = "menuToolsInsertParentheses";
         this.menuToolsInsertParentheses.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D9)));
         this.menuToolsInsertParentheses.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertParentheses.Text = "Insert Parentheses";
         this.menuToolsInsertParentheses.Click += new System.EventHandler(this.menuToolsInsertParentheses_Click);
         // 
         // menuToolsInsertOneLineClosure
         // 
         this.menuToolsInsertOneLineClosure.Name = "menuToolsInsertOneLineClosure";
         this.menuToolsInsertOneLineClosure.ShortcutKeys = System.Windows.Forms.Keys.F3;
         this.menuToolsInsertOneLineClosure.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertOneLineClosure.Text = "Insert One Line Closure";
         this.menuToolsInsertOneLineClosure.Click += new System.EventHandler(this.menuToolsInsertOneLineClosure_Click);
         // 
         // menuToolsInsertOneLineClosureArguments
         // 
         this.menuToolsInsertOneLineClosureArguments.Name = "menuToolsInsertOneLineClosureArguments";
         this.menuToolsInsertOneLineClosureArguments.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
         this.menuToolsInsertOneLineClosureArguments.Size = new System.Drawing.Size(333, 22);
         this.menuToolsInsertOneLineClosureArguments.Text = "Insert One Line Closure (Arguments)";
         this.menuToolsInsertOneLineClosureArguments.Click += new System.EventHandler(this.menuToolsInsertOneLineClosureArguments_Click);
         // 
         // menuToolsConvertToOneLineClosure
         // 
         this.menuToolsConvertToOneLineClosure.Name = "menuToolsConvertToOneLineClosure";
         this.menuToolsConvertToOneLineClosure.ShortcutKeys = System.Windows.Forms.Keys.F4;
         this.menuToolsConvertToOneLineClosure.Size = new System.Drawing.Size(333, 22);
         this.menuToolsConvertToOneLineClosure.Text = "Convert to One Line Closure";
         this.menuToolsConvertToOneLineClosure.Click += new System.EventHandler(this.menuToolsConvertToOneLineClosure_Click);
         // 
         // menuToolsConvertToStandardClosure
         // 
         this.menuToolsConvertToStandardClosure.Name = "menuToolsConvertToStandardClosure";
         this.menuToolsConvertToStandardClosure.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
         this.menuToolsConvertToStandardClosure.Size = new System.Drawing.Size(333, 22);
         this.menuToolsConvertToStandardClosure.Text = "Convert to Standard Closure";
         this.menuToolsConvertToStandardClosure.Click += new System.EventHandler(this.menuToolsConvertToStandardClosure_Click);
         // 
         // menuLanguage
         // 
         this.menuLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLanguageRefresh,
            this.menuLanguageManual,
            this.menuLanguageVerbose,
            this.menuLanguageAutoVariables,
            this.menuLanguageFancy,
            this.menuLanguageBuild});
         this.menuLanguage.Name = "menuLanguage";
         this.menuLanguage.Size = new System.Drawing.Size(71, 20);
         this.menuLanguage.Text = "&Language";
         // 
         // menuLanguageRefresh
         // 
         this.menuLanguageRefresh.Name = "menuLanguageRefresh";
         this.menuLanguageRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
         this.menuLanguageRefresh.Size = new System.Drawing.Size(189, 22);
         this.menuLanguageRefresh.Text = "&Refresh";
         this.menuLanguageRefresh.Click += new System.EventHandler(this.menuLanguageRefresh_Click);
         // 
         // menuLanguageManual
         // 
         this.menuLanguageManual.Name = "menuLanguageManual";
         this.menuLanguageManual.ShortcutKeys = System.Windows.Forms.Keys.F9;
         this.menuLanguageManual.Size = new System.Drawing.Size(189, 22);
         this.menuLanguageManual.Text = "&Manual";
         this.menuLanguageManual.Click += new System.EventHandler(this.menuLanguageManual_Click);
         // 
         // menuLanguageVerbose
         // 
         this.menuLanguageVerbose.Name = "menuLanguageVerbose";
         this.menuLanguageVerbose.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.V)));
         this.menuLanguageVerbose.Size = new System.Drawing.Size(189, 22);
         this.menuLanguageVerbose.Text = "Verbose";
         this.menuLanguageVerbose.Click += new System.EventHandler(this.menuLanguageVerbose_Click);
         // 
         // menuLanguageAutoVariables
         // 
         this.menuLanguageAutoVariables.Name = "menuLanguageAutoVariables";
         this.menuLanguageAutoVariables.ShortcutKeys = System.Windows.Forms.Keys.F8;
         this.menuLanguageAutoVariables.Size = new System.Drawing.Size(189, 22);
         this.menuLanguageAutoVariables.Text = "&Auto-Variables";
         this.menuLanguageAutoVariables.Click += new System.EventHandler(this.menuLanguageAutoVariables_Click);
         // 
         // menuLanguageFancy
         // 
         this.menuLanguageFancy.Name = "menuLanguageFancy";
         this.menuLanguageFancy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F8)));
         this.menuLanguageFancy.Size = new System.Drawing.Size(189, 22);
         this.menuLanguageFancy.Text = "Fancy";
         this.menuLanguageFancy.Click += new System.EventHandler(this.menuLanguageFancy_Click);
         // 
         // menuLanguageBuild
         // 
         this.menuLanguageBuild.Name = "menuLanguageBuild";
         this.menuLanguageBuild.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
         this.menuLanguageBuild.Size = new System.Drawing.Size(189, 22);
         this.menuLanguageBuild.Text = "&Build";
         this.menuLanguageBuild.Click += new System.EventHandler(this.menuLanguageBuild_Click);
         // 
         // menuRefactor
         // 
         this.menuRefactor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRefactorVariable,
            this.menuRefactorRename});
         this.menuRefactor.Name = "menuRefactor";
         this.menuRefactor.Size = new System.Drawing.Size(63, 20);
         this.menuRefactor.Text = "&Refactor";
         // 
         // menuRefactorVariable
         // 
         this.menuRefactorVariable.Name = "menuRefactorVariable";
         this.menuRefactorVariable.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.V)));
         this.menuRefactorVariable.Size = new System.Drawing.Size(186, 22);
         this.menuRefactorVariable.Text = "&Variable";
         this.menuRefactorVariable.Click += new System.EventHandler(this.menuRefactorVariable_Click);
         // 
         // menuRefactorRename
         // 
         this.menuRefactorRename.Name = "menuRefactorRename";
         this.menuRefactorRename.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.R)));
         this.menuRefactorRename.Size = new System.Drawing.Size(186, 22);
         this.menuRefactorRename.Text = "&Rename";
         this.menuRefactorRename.Click += new System.EventHandler(this.menuRefactorRename_Click);
         // 
         // menuDebug
         // 
         this.menuDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDebugEnabled,
            this.menuDebugBreakpoint,
            this.menuDebugStep,
            this.menuDebugStepInto});
         this.menuDebug.Name = "menuDebug";
         this.menuDebug.Size = new System.Drawing.Size(54, 20);
         this.menuDebug.Text = "&Debug";
         // 
         // menuDebugEnabled
         // 
         this.menuDebugEnabled.Name = "menuDebugEnabled";
         this.menuDebugEnabled.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.D)));
         this.menuDebugEnabled.Size = new System.Drawing.Size(190, 22);
         this.menuDebugEnabled.Text = "&Enabled";
         this.menuDebugEnabled.Click += new System.EventHandler(this.menuDebugEnabled_Click);
         // 
         // menuDebugBreakpoint
         // 
         this.menuDebugBreakpoint.Name = "menuDebugBreakpoint";
         this.menuDebugBreakpoint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Insert)));
         this.menuDebugBreakpoint.Size = new System.Drawing.Size(190, 22);
         this.menuDebugBreakpoint.Text = "&Breakpoint";
         this.menuDebugBreakpoint.Click += new System.EventHandler(this.menuDebugBreakpoint_Click);
         // 
         // menuDebugStep
         // 
         this.menuDebugStep.Name = "menuDebugStep";
         this.menuDebugStep.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.End)));
         this.menuDebugStep.Size = new System.Drawing.Size(190, 22);
         this.menuDebugStep.Text = "&Step";
         this.menuDebugStep.Click += new System.EventHandler(this.menuDebugStep_Click);
         // 
         // menuDebugStepInto
         // 
         this.menuDebugStepInto.Name = "menuDebugStepInto";
         this.menuDebugStepInto.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down)));
         this.menuDebugStepInto.Size = new System.Drawing.Size(190, 22);
         this.menuDebugStepInto.Text = "Step &Into";
         this.menuDebugStepInto.Click += new System.EventHandler(this.menuDebugStepInto_Click);
         // 
         // table
         // 
         this.table.ColumnCount = 1;
         this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.table.Controls.Add(this.richCode, 0, 0);
         this.table.Controls.Add(this.status, 0, 3);
         this.table.Controls.Add(this.tabConsole, 0, 2);
         this.table.Controls.Add(this.labelValue, 0, 1);
         this.table.Dock = System.Windows.Forms.DockStyle.Fill;
         this.table.Location = new System.Drawing.Point(0, 24);
         this.table.Name = "table";
         this.table.RowCount = 4;
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
         this.table.Size = new System.Drawing.Size(671, 644);
         this.table.TabIndex = 1;
         // 
         // richCode
         // 
         this.richCode.AcceptsTab = true;
         this.richCode.ContextMenuStrip = this.menuCode;
         this.richCode.DetectUrls = false;
         this.richCode.Dock = System.Windows.Forms.DockStyle.Fill;
         this.richCode.Font = new System.Drawing.Font("Anonymous Pro", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.richCode.HideSelection = false;
         this.richCode.Location = new System.Drawing.Point(3, 3);
         this.richCode.Name = "richCode";
         this.richCode.ShowSelectionMargin = true;
         this.richCode.Size = new System.Drawing.Size(665, 276);
         this.richCode.TabIndex = 0;
         this.richCode.Text = "";
         this.richCode.WordWrap = false;
         this.richCode.TextChanged += new System.EventHandler(this.richCode_TextChanged);
         this.richCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richCode_KeyPress);
         this.richCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.richCode_KeyUp);
         // 
         // menuCode
         // 
         this.menuCode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCodePasteDoubleQuotedText,
            this.menuCodePasteSingleQuotedText});
         this.menuCode.Name = "menuCode";
         this.menuCode.Size = new System.Drawing.Size(279, 48);
         // 
         // menuCodePasteDoubleQuotedText
         // 
         this.menuCodePasteDoubleQuotedText.Name = "menuCodePasteDoubleQuotedText";
         this.menuCodePasteDoubleQuotedText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.D)));
         this.menuCodePasteDoubleQuotedText.Size = new System.Drawing.Size(278, 22);
         this.menuCodePasteDoubleQuotedText.Text = "Paste &Double-Quoted Text";
         this.menuCodePasteDoubleQuotedText.Click += new System.EventHandler(this.menuCodePasteDoubleQuotedText_Click);
         // 
         // menuCodePasteSingleQuotedText
         // 
         this.menuCodePasteSingleQuotedText.Name = "menuCodePasteSingleQuotedText";
         this.menuCodePasteSingleQuotedText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.S)));
         this.menuCodePasteSingleQuotedText.Size = new System.Drawing.Size(278, 22);
         this.menuCodePasteSingleQuotedText.Text = "Paste &Single-Quoted Text";
         this.menuCodePasteSingleQuotedText.Click += new System.EventHandler(this.menuCodePasteSingleQuotedText_Click);
         // 
         // status
         // 
         this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
         this.status.Location = new System.Drawing.Point(0, 624);
         this.status.Name = "status";
         this.status.Size = new System.Drawing.Size(671, 20);
         this.status.TabIndex = 2;
         this.status.Text = "statusStrip1";
         // 
         // labelStatus
         // 
         this.labelStatus.BackColor = System.Drawing.SystemColors.Info;
         this.labelStatus.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelStatus.Name = "labelStatus";
         this.labelStatus.Size = new System.Drawing.Size(656, 15);
         this.labelStatus.Spring = true;
         this.labelStatus.Text = "Ready";
         this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // tabConsole
         // 
         this.tabConsole.Controls.Add(this.tabPage1);
         this.tabConsole.Controls.Add(this.tabPage3);
         this.tabConsole.Controls.Add(this.tabPage2);
         this.tabConsole.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabConsole.Location = new System.Drawing.Point(3, 345);
         this.tabConsole.Name = "tabConsole";
         this.tabConsole.SelectedIndex = 0;
         this.tabConsole.Size = new System.Drawing.Size(665, 276);
         this.tabConsole.TabIndex = 3;
         // 
         // tabPage1
         // 
         this.tabPage1.Controls.Add(this.richConsole);
         this.tabPage1.Location = new System.Drawing.Point(4, 22);
         this.tabPage1.Name = "tabPage1";
         this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage1.Size = new System.Drawing.Size(657, 250);
         this.tabPage1.TabIndex = 0;
         this.tabPage1.Text = "Console";
         this.tabPage1.UseVisualStyleBackColor = true;
         // 
         // richConsole
         // 
         this.richConsole.AcceptsTab = true;
         this.richConsole.BackColor = System.Drawing.Color.Blue;
         this.richConsole.ContextMenuStrip = this.menuConsole;
         this.richConsole.DetectUrls = false;
         this.richConsole.Dock = System.Windows.Forms.DockStyle.Fill;
         this.richConsole.Font = new System.Drawing.Font("Source Code Pro", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.richConsole.ForeColor = System.Drawing.Color.Gold;
         this.richConsole.HideSelection = false;
         this.richConsole.Location = new System.Drawing.Point(3, 3);
         this.richConsole.Name = "richConsole";
         this.richConsole.ReadOnly = true;
         this.richConsole.Size = new System.Drawing.Size(651, 244);
         this.richConsole.TabIndex = 2;
         this.richConsole.Text = "";
         this.richConsole.WordWrap = false;
         this.richConsole.SelectionChanged += new System.EventHandler(this.richConsole_SelectionChanged);
         // 
         // menuConsole
         // 
         this.menuConsole.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConsoleCopyToClipboard,
            this.menuConsoleCopyToEditor,
            this.menuConsoleSaveToFile});
         this.menuConsole.Name = "menuConsole";
         this.menuConsole.Size = new System.Drawing.Size(246, 70);
         // 
         // menuConsoleCopyToClipboard
         // 
         this.menuConsoleCopyToClipboard.Name = "menuConsoleCopyToClipboard";
         this.menuConsoleCopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.C)));
         this.menuConsoleCopyToClipboard.Size = new System.Drawing.Size(245, 22);
         this.menuConsoleCopyToClipboard.Text = "&Copy to Clipboard";
         this.menuConsoleCopyToClipboard.Click += new System.EventHandler(this.menuConsoleCopyToClipboard_Click);
         // 
         // menuConsoleCopyToEditor
         // 
         this.menuConsoleCopyToEditor.Name = "menuConsoleCopyToEditor";
         this.menuConsoleCopyToEditor.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.E)));
         this.menuConsoleCopyToEditor.Size = new System.Drawing.Size(245, 22);
         this.menuConsoleCopyToEditor.Text = "Copy to &Editor";
         this.menuConsoleCopyToEditor.Click += new System.EventHandler(this.menuConsoleCopyToEditor_Click);
         // 
         // menuConsoleSaveToFile
         // 
         this.menuConsoleSaveToFile.Name = "menuConsoleSaveToFile";
         this.menuConsoleSaveToFile.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
         | System.Windows.Forms.Keys.S)));
         this.menuConsoleSaveToFile.Size = new System.Drawing.Size(245, 22);
         this.menuConsoleSaveToFile.Text = "&Save to File";
         this.menuConsoleSaveToFile.Click += new System.EventHandler(this.menuConsoleSaveToFile_Click);
         // 
         // tabPage3
         // 
         this.tabPage3.Controls.Add(this.textText);
         this.tabPage3.Location = new System.Drawing.Point(4, 22);
         this.tabPage3.Name = "tabPage3";
         this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage3.Size = new System.Drawing.Size(657, 250);
         this.tabPage3.TabIndex = 2;
         this.tabPage3.Text = "Text";
         this.tabPage3.UseVisualStyleBackColor = true;
         // 
         // textText
         // 
         this.textText.AcceptsReturn = true;
         this.textText.AcceptsTab = true;
         this.textText.ContextMenuStrip = this.menuText;
         this.textText.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textText.Font = new System.Drawing.Font("Source Code Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.textText.Location = new System.Drawing.Point(3, 3);
         this.textText.Multiline = true;
         this.textText.Name = "textText";
         this.textText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.textText.Size = new System.Drawing.Size(651, 244);
         this.textText.TabIndex = 0;
         this.textText.WordWrap = false;
         // 
         // menuText
         // 
         this.menuText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTextCut,
            this.menuTextCopy,
            this.menuTextPaste,
            this.menuTextClear,
            this.menuTextOpen,
            this.menuTextSetText,
            this.menuTextGetText});
         this.menuText.Name = "menuText";
         this.menuText.Size = new System.Drawing.Size(183, 158);
         // 
         // menuTextCut
         // 
         this.menuTextCut.Name = "menuTextCut";
         this.menuTextCut.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.X)));
         this.menuTextCut.Size = new System.Drawing.Size(182, 22);
         this.menuTextCut.Text = "Cut";
         this.menuTextCut.Click += new System.EventHandler(this.menuTextCut_Click);
         // 
         // menuTextCopy
         // 
         this.menuTextCopy.Name = "menuTextCopy";
         this.menuTextCopy.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.C)));
         this.menuTextCopy.Size = new System.Drawing.Size(182, 22);
         this.menuTextCopy.Text = "Copy";
         this.menuTextCopy.Click += new System.EventHandler(this.menuTextCopy_Click);
         // 
         // menuTextPaste
         // 
         this.menuTextPaste.Name = "menuTextPaste";
         this.menuTextPaste.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.V)));
         this.menuTextPaste.Size = new System.Drawing.Size(182, 22);
         this.menuTextPaste.Text = "Paste";
         this.menuTextPaste.Click += new System.EventHandler(this.menuTextPaste_Click);
         // 
         // menuTextClear
         // 
         this.menuTextClear.Name = "menuTextClear";
         this.menuTextClear.Size = new System.Drawing.Size(182, 22);
         this.menuTextClear.Text = "&Clear";
         this.menuTextClear.Click += new System.EventHandler(this.menuTextClear_Click);
         // 
         // menuTextOpen
         // 
         this.menuTextOpen.Name = "menuTextOpen";
         this.menuTextOpen.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.O)));
         this.menuTextOpen.Size = new System.Drawing.Size(182, 22);
         this.menuTextOpen.Text = "&Open";
         this.menuTextOpen.Click += new System.EventHandler(this.menuTextOpen_Click);
         // 
         // menuTextSetText
         // 
         this.menuTextSetText.Name = "menuTextSetText";
         this.menuTextSetText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.T)));
         this.menuTextSetText.Size = new System.Drawing.Size(182, 22);
         this.menuTextSetText.Text = "Set &Text";
         this.menuTextSetText.Click += new System.EventHandler(this.menuTextSetText_Click);
         // 
         // menuTextGetText
         // 
         this.menuTextGetText.Name = "menuTextGetText";
         this.menuTextGetText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
         | System.Windows.Forms.Keys.G)));
         this.menuTextGetText.Size = new System.Drawing.Size(182, 22);
         this.menuTextGetText.Text = "&Get Text";
         this.menuTextGetText.Click += new System.EventHandler(this.menuTextGetText_Click);
         // 
         // tabPage2
         // 
         this.tabPage2.Controls.Add(this.webBrowser);
         this.tabPage2.Location = new System.Drawing.Point(4, 22);
         this.tabPage2.Name = "tabPage2";
         this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage2.Size = new System.Drawing.Size(657, 250);
         this.tabPage2.TabIndex = 1;
         this.tabPage2.Text = "Web";
         this.tabPage2.UseVisualStyleBackColor = true;
         // 
         // webBrowser
         // 
         this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
         this.webBrowser.Location = new System.Drawing.Point(3, 3);
         this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
         this.webBrowser.Name = "webBrowser";
         this.webBrowser.Size = new System.Drawing.Size(651, 244);
         this.webBrowser.TabIndex = 0;
         // 
         // labelValue
         // 
         this.labelValue.AutoEllipsis = true;
         this.labelValue.BackColor = System.Drawing.SystemColors.Info;
         this.labelValue.Dock = System.Windows.Forms.DockStyle.Fill;
         this.labelValue.Font = new System.Drawing.Font("Segoe Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelValue.ForeColor = System.Drawing.SystemColors.InfoText;
         this.labelValue.Location = new System.Drawing.Point(3, 282);
         this.labelValue.Name = "labelValue";
         this.labelValue.Size = new System.Drawing.Size(665, 60);
         this.labelValue.TabIndex = 4;
         this.labelValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         this.labelValue.UseMnemonic = false;
         // 
         // dialogOpen
         // 
         this.dialogOpen.DefaultExt = "orange";
         this.dialogOpen.Filter = "Orange files (*.orange)|*.orange|All files (*.*)|*.*";
         this.dialogOpen.InitialDirectory = "C:\\Enterprise\\Orange";
         this.dialogOpen.Title = "Open Orange Source Code";
         // 
         // dialogSave
         // 
         this.dialogSave.DefaultExt = "orange";
         this.dialogSave.Filter = "Orange files (*.orange)|*.orange|All files (*.*)|*.*";
         this.dialogSave.InitialDirectory = "C:\\Enterprise\\Orange";
         this.dialogSave.RestoreDirectory = true;
         this.dialogSave.Title = "Save Orange Source Code";
         // 
         // dialogFolder
         // 
         this.dialogFolder.Description = "Set Working FOlder";
         // 
         // dialogSaveToFile
         // 
         this.dialogSaveToFile.DefaultExt = "txt";
         this.dialogSaveToFile.Filter = "Text files (*.txt)|*.txt|XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv|All file" +
 "s (*.*)|*.*";
         this.dialogSaveToFile.Title = "Save Console";
         // 
         // dialogOpenText
         // 
         this.dialogOpenText.DefaultExt = "txt";
         this.dialogOpenText.Filter = "Text files (*.txt)|*.txt|XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv|Other (*" +
 ".*)|*.*";
         this.dialogOpenText.Title = "Assign to $0";
         // 
         // OrangeIDE
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(671, 668);
         this.Controls.Add(this.table);
         this.Controls.Add(this.menu);
         this.MainMenuStrip = this.menu;
         this.Name = "OrangeIDE";
         this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
         this.Text = "Orange IDE";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrangeIDE_FormClosing);
         this.Load += new System.EventHandler(this.OrangeIDE_Load);
         this.menu.ResumeLayout(false);
         this.menu.PerformLayout();
         this.table.ResumeLayout(false);
         this.table.PerformLayout();
         this.menuCode.ResumeLayout(false);
         this.status.ResumeLayout(false);
         this.status.PerformLayout();
         this.tabConsole.ResumeLayout(false);
         this.tabPage1.ResumeLayout(false);
         this.menuConsole.ResumeLayout(false);
         this.tabPage3.ResumeLayout(false);
         this.tabPage3.PerformLayout();
         this.menuText.ResumeLayout(false);
         this.tabPage2.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menu;
      private System.Windows.Forms.ToolStripMenuItem menuFile;
      private System.Windows.Forms.ToolStripMenuItem menuFileNew;
      private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
      private System.Windows.Forms.ToolStripMenuItem menuFileSave;
      private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
      private System.Windows.Forms.ToolStripMenuItem menuEdit;
      private System.Windows.Forms.ToolStripMenuItem menuEditUndo;
      private System.Windows.Forms.ToolStripMenuItem menuEditRedo;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem menuEditCut;
      private System.Windows.Forms.ToolStripMenuItem menuEditCopy;
      private System.Windows.Forms.ToolStripMenuItem menuEditPaste;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
      private System.Windows.Forms.ToolStripMenuItem menuEditSelectAll;
      private System.Windows.Forms.ToolStripMenuItem menuTools;
      private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem menuLanguage;
      private System.Windows.Forms.ToolStripMenuItem menuLanguageRefresh;
      private System.Windows.Forms.ToolStripMenuItem menuLanguageManual;
      private System.Windows.Forms.TableLayoutPanel table;
      private System.Windows.Forms.RichTextBox richCode;
      private System.Windows.Forms.OpenFileDialog dialogOpen;
      private System.Windows.Forms.SaveFileDialog dialogSave;
      private System.Windows.Forms.StatusStrip status;
      private System.Windows.Forms.ToolStripStatusLabel labelStatus;
      private System.Windows.Forms.ToolStripMenuItem menuLanguageVerbose;
      private System.Windows.Forms.ToolStripMenuItem menuToolsSetWorkingFolder;
      private System.Windows.Forms.FolderBrowserDialog dialogFolder;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertQuotes;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertPrint;
      private System.Windows.Forms.ToolStripMenuItem menuLanguageAutoVariables;
      private System.Windows.Forms.ContextMenuStrip menuCode;
      private System.Windows.Forms.ToolStripMenuItem menuCodePasteDoubleQuotedText;
      private System.Windows.Forms.ToolStripMenuItem menuCodePasteSingleQuotedText;
      private System.Windows.Forms.ContextMenuStrip menuConsole;
      private System.Windows.Forms.ToolStripMenuItem menuConsoleCopyToClipboard;
      private System.Windows.Forms.ToolStripMenuItem menuConsoleCopyToEditor;
      private System.Windows.Forms.ToolStripMenuItem menuConsoleSaveToFile;
      private System.Windows.Forms.SaveFileDialog dialogSaveToFile;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertInterpolationQuotes;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertPatternDelimiters;
      private System.Windows.Forms.ToolStripMenuItem menuToolsGetFile;
      private System.Windows.Forms.OpenFileDialog dialogOpenText;
      private System.Windows.Forms.TabControl tabConsole;
      private System.Windows.Forms.TabPage tabPage1;
      private System.Windows.Forms.RichTextBox richConsole;
      private System.Windows.Forms.TabPage tabPage2;
      private System.Windows.Forms.WebBrowser webBrowser;
      private System.Windows.Forms.TabPage tabPage3;
      private System.Windows.Forms.TextBox textText;
      private System.Windows.Forms.ContextMenuStrip menuText;
      private System.Windows.Forms.ToolStripMenuItem menuTextCut;
      private System.Windows.Forms.ToolStripMenuItem menuTextCopy;
      private System.Windows.Forms.ToolStripMenuItem menuTextPaste;
      private System.Windows.Forms.ToolStripMenuItem menuTextClear;
      private System.Windows.Forms.ToolStripMenuItem menuTextOpen;
      private System.Windows.Forms.ToolStripMenuItem menuTextSetText;
      private System.Windows.Forms.ToolStripMenuItem menuTextGetText;
      private System.Windows.Forms.Label labelValue;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem menuEditCodeAsText;
      private System.Windows.Forms.ToolStripMenuItem menuEditSaveTextToCode;
      private System.Windows.Forms.ToolStripMenuItem menuEditAbandonCodeChanges;
      private System.Windows.Forms.ToolStripMenuItem menuInsertInterpolationQuotes;
      private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem menuEditDuplicate;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertRecs;
      private System.Windows.Forms.ToolStripMenuItem menuLanguageFancy;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertOutQ;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertEmptyArrayLiteral;
      private System.Windows.Forms.ToolStripMenuItem menuRefactor;
      private System.Windows.Forms.ToolStripMenuItem menuRefactorVariable;
      private System.Windows.Forms.ToolStripMenuItem menuRefactorRename;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertPrintBlock;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertClassDelimiters;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertBlock;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertClosure;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertSingleQuotes;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertParentheses;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertOneLineClosure;
      private System.Windows.Forms.ToolStripMenuItem menuToolsInsertOneLineClosureArguments;
      private System.Windows.Forms.ToolStripMenuItem menuToolsConvertToOneLineClosure;
      private System.Windows.Forms.ToolStripMenuItem menuToolsConvertToStandardClosure;
      private System.Windows.Forms.ToolStripMenuItem menuLanguageBuild;
      private System.Windows.Forms.ToolStripMenuItem menuDebug;
      private System.Windows.Forms.ToolStripMenuItem menuDebugEnabled;
      private System.Windows.Forms.ToolStripMenuItem menuDebugBreakpoint;
      private System.Windows.Forms.ToolStripMenuItem menuDebugStep;
      private System.Windows.Forms.ToolStripMenuItem menuDebugStepInto;
   }
}

