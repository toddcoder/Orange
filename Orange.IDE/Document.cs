using System.Text;
using System.Windows.Forms;
using Standard.Computer;
using Standard.Types.Enumerables;
using Standard.Types.RegularExpressions;

namespace Orange.IDE
{
   public class Document
   {
      Form form;
      RichTextBox richTextBox;
      bool isDirty;
      string formName;
      FileName file;

      public Document(Form form, RichTextBox richTextBox)
      {
         this.form = form;
         this.richTextBox = richTextBox;
         isDirty = false;
         formName = this.form.Text;
         file = null;
      }

      public string FileName => file != null ? file.ToString() : "";

      public FileName File => file;

      public OpenFileDialog OpenFileDialog { get; set; }

      public SaveFileDialog SaveFileDialog { get; set; }

      public bool IsDirty
      {
         get => isDirty;
         set => isDirty = value;
      }

      public void Dirty() => isDirty = true;

      public void Clean() => isDirty = false;

      public void New()
      {
         if (isDirty)
         {
            var result = getSaveResponse();
            switch (result)
            {
               case DialogResult.Yes:
                  Save();
                  break;
               case DialogResult.No:
                  richTextBox.Clear();
                  isDirty = false;
                  break;
               default:
                  return;
            }
         }

         richTextBox.Clear();
         isDirty = false;
         file = null;
      }

      public void Open()
      {
         if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            Open(OpenFileDialog.FileName);
      }

      public void Open(FileName fileName)
      {
         file = fileName;
         richTextBox.Text = fileName.Lines.Listify("\r\n");
         isDirty = false;
         DisplayFileName();
      }

      public void DisplayFileName()
      {
         var title = new StringBuilder();
         if (file != null)
         {
            title.Append(file);
            title.Append(" - ");
            title.Append(formName);
            if (isDirty)
               title.Append(" *");
         }
      }

      string getText() => richTextBox.Text.Split("[\r\n]").Listify("\r\n");

      public void Save()
      {
         if (isDirty)
            if (file != null)
            {
               if (file.Exists())
                  file.Delete();
               file.Encoding = Encoding.UTF8;
               file.Text = getText();
               isDirty = false;
               DisplayFileName();
            }
            else
               SaveAs();
      }

      public void SaveAs()
      {
         if (SaveFileDialog.ShowDialog() == DialogResult.OK)
         {
            file = SaveFileDialog.FileName;
            if (file.Exists())
               file.Delete();
            file.Encoding = Encoding.UTF8;
            file.Text = getText();
            isDirty = false;
            DisplayFileName();
         }
      }

      public void Close(FormClosingEventArgs e)
      {
         if (isDirty)
         {
            var result = getSaveResponse();
            switch (result)
            {
               case DialogResult.Yes:
                  Save();
                  break;
               case DialogResult.Cancel:
                  e.Cancel = true;
                  break;
            }
         }
      }

      DialogResult getSaveResponse()
      {
         var message = file == null ? "File not saved" : "File " + file + " not saved";
         return MessageBox.Show(message, "Orange File Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
      }
   }
}