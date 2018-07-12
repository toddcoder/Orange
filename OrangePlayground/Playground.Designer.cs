namespace OrangePlayground
{
   partial class Playground
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Playground));
         this.table = new System.Windows.Forms.TableLayoutPanel();
         this.tabs = new System.Windows.Forms.TabControl();
         this.tabConsole = new System.Windows.Forms.TabPage();
         this.textConsole = new System.Windows.Forms.RichTextBox();
         this.tabErrors = new System.Windows.Forms.TabPage();
         this.textErrors = new System.Windows.Forms.RichTextBox();
         this.tabText = new System.Windows.Forms.TabPage();
         this.textText = new System.Windows.Forms.RichTextBox();
         this.labelResult = new System.Windows.Forms.Label();
         this.panel1 = new System.Windows.Forms.Panel();
         this.textEditor = new OrangePlayground.DrawableRichTextBox();
         this.statusStrip = new System.Windows.Forms.StatusStrip();
         this.labalElapsed = new System.Windows.Forms.ToolStripStatusLabel();
         this.dialogSave = new System.Windows.Forms.SaveFileDialog();
         this.table.SuspendLayout();
         this.tabs.SuspendLayout();
         this.tabConsole.SuspendLayout();
         this.tabErrors.SuspendLayout();
         this.tabText.SuspendLayout();
         this.panel1.SuspendLayout();
         this.statusStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // table
         // 
         this.table.ColumnCount = 1;
         this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.table.Controls.Add(this.tabs, 0, 2);
         this.table.Controls.Add(this.labelResult, 0, 1);
         this.table.Controls.Add(this.panel1, 0, 0);
         this.table.Dock = System.Windows.Forms.DockStyle.Fill;
         this.table.Location = new System.Drawing.Point(0, 0);
         this.table.Name = "table";
         this.table.RowCount = 3;
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
         this.table.Size = new System.Drawing.Size(805, 664);
         this.table.TabIndex = 0;
         // 
         // tabs
         // 
         this.tabs.Controls.Add(this.tabConsole);
         this.tabs.Controls.Add(this.tabErrors);
         this.tabs.Controls.Add(this.tabText);
         this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabs.Location = new System.Drawing.Point(3, 365);
         this.tabs.Name = "tabs";
         this.tabs.SelectedIndex = 0;
         this.tabs.Size = new System.Drawing.Size(799, 296);
         this.tabs.TabIndex = 3;
         // 
         // tabConsole
         // 
         this.tabConsole.Controls.Add(this.textConsole);
         this.tabConsole.Location = new System.Drawing.Point(4, 22);
         this.tabConsole.Name = "tabConsole";
         this.tabConsole.Padding = new System.Windows.Forms.Padding(3);
         this.tabConsole.Size = new System.Drawing.Size(791, 270);
         this.tabConsole.TabIndex = 0;
         this.tabConsole.Text = "Console";
         this.tabConsole.UseVisualStyleBackColor = true;
         // 
         // textConsole
         // 
         this.textConsole.DetectUrls = false;
         this.textConsole.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textConsole.Location = new System.Drawing.Point(3, 3);
         this.textConsole.Name = "textConsole";
         this.textConsole.Size = new System.Drawing.Size(785, 264);
         this.textConsole.TabIndex = 2;
         this.textConsole.Text = "";
         // 
         // tabErrors
         // 
         this.tabErrors.Controls.Add(this.textErrors);
         this.tabErrors.Location = new System.Drawing.Point(4, 22);
         this.tabErrors.Name = "tabErrors";
         this.tabErrors.Padding = new System.Windows.Forms.Padding(3);
         this.tabErrors.Size = new System.Drawing.Size(791, 270);
         this.tabErrors.TabIndex = 1;
         this.tabErrors.Text = "Errors";
         this.tabErrors.UseVisualStyleBackColor = true;
         // 
         // textErrors
         // 
         this.textErrors.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textErrors.Location = new System.Drawing.Point(3, 3);
         this.textErrors.Name = "textErrors";
         this.textErrors.Size = new System.Drawing.Size(785, 264);
         this.textErrors.TabIndex = 3;
         this.textErrors.Text = "";
         // 
         // tabText
         // 
         this.tabText.Controls.Add(this.textText);
         this.tabText.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.tabText.Location = new System.Drawing.Point(4, 22);
         this.tabText.Name = "tabText";
         this.tabText.Size = new System.Drawing.Size(791, 270);
         this.tabText.TabIndex = 2;
         this.tabText.Text = "Text";
         this.tabText.UseVisualStyleBackColor = true;
         // 
         // textText
         // 
         this.textText.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textText.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.textText.Location = new System.Drawing.Point(0, 0);
         this.textText.Name = "textText";
         this.textText.Size = new System.Drawing.Size(791, 270);
         this.textText.TabIndex = 4;
         this.textText.Text = "";
         // 
         // labelResult
         // 
         this.labelResult.AutoSize = true;
         this.labelResult.BackColor = System.Drawing.SystemColors.Highlight;
         this.labelResult.Dock = System.Windows.Forms.DockStyle.Fill;
         this.labelResult.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelResult.ForeColor = System.Drawing.SystemColors.HighlightText;
         this.labelResult.Location = new System.Drawing.Point(3, 302);
         this.labelResult.Name = "labelResult";
         this.labelResult.Size = new System.Drawing.Size(799, 60);
         this.labelResult.TabIndex = 4;
         this.labelResult.Text = "...";
         this.labelResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.textEditor);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel1.Location = new System.Drawing.Point(3, 3);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(799, 296);
         this.panel1.TabIndex = 5;
         // 
         // textEditor
         // 
         this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textEditor.Location = new System.Drawing.Point(0, 0);
         this.textEditor.Name = "textEditor";
         this.textEditor.Size = new System.Drawing.Size(799, 296);
         this.textEditor.TabIndex = 1;
         this.textEditor.Text = "";
         this.textEditor.SelectionChanged += new System.EventHandler(this.textResults_SelectionChanged);
         this.textEditor.VScroll += new System.EventHandler(this.textEditor_VScroll);
         this.textEditor.TextChanged += new System.EventHandler(this.textEditor_TextChanged);
         this.textEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEditor_KeyPress);
         this.textEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEditor_KeyUp);
         // 
         // statusStrip
         // 
         this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labalElapsed});
         this.statusStrip.Location = new System.Drawing.Point(0, 642);
         this.statusStrip.Name = "statusStrip";
         this.statusStrip.Size = new System.Drawing.Size(805, 22);
         this.statusStrip.TabIndex = 1;
         // 
         // labalElapsed
         // 
         this.labalElapsed.Name = "labalElapsed";
         this.labalElapsed.Size = new System.Drawing.Size(118, 17);
         this.labalElapsed.Text = "toolStripStatusLabel1";
         // 
         // dialogSave
         // 
         this.dialogSave.DefaultExt = "*.txt";
         this.dialogSave.Filter = "Text files|*.txt|All files|*.*";
         this.dialogSave.Title = "Save Text";
         // 
         // Playground
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(805, 664);
         this.Controls.Add(this.statusStrip);
         this.Controls.Add(this.table);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "Playground";
         this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
         this.Text = "Orange Playground";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Playground_FormClosing);
         this.Load += new System.EventHandler(this.Playground_Load);
         this.table.ResumeLayout(false);
         this.table.PerformLayout();
         this.tabs.ResumeLayout(false);
         this.tabConsole.ResumeLayout(false);
         this.tabErrors.ResumeLayout(false);
         this.tabText.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.statusStrip.ResumeLayout(false);
         this.statusStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel table;
      private System.Windows.Forms.TabControl tabs;
      private System.Windows.Forms.TabPage tabConsole;
      private System.Windows.Forms.RichTextBox textConsole;
      private System.Windows.Forms.TabPage tabErrors;
      private System.Windows.Forms.RichTextBox textErrors;
      private System.Windows.Forms.Label labelResult;
      private System.Windows.Forms.TabPage tabText;
      private System.Windows.Forms.RichTextBox textText;
      private System.Windows.Forms.StatusStrip statusStrip;
      private System.Windows.Forms.ToolStripStatusLabel labalElapsed;
      private System.Windows.Forms.SaveFileDialog dialogSave;
      private System.Windows.Forms.Panel panel1;
      private DrawableRichTextBox textEditor;
   }
}

