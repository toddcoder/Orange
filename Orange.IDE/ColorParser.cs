using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Orange.Library;
using Orange.Library.Parsers;
using Standard.Types.Collections;

namespace Orange.IDE
{
	public class ColorParser : IColorizer
	{
		public static void StopTextBoxUpdate(RichTextBox textBox)
		{
			SendMessage(textBox.Handle, WM_SETREDRAW, false, 0);
		}

		public static void ResumeTextBoxUpdate(RichTextBox textBox)
		{
			SendMessage(textBox.Handle, WM_SETREDRAW, true, 0);
		}

		const int WM_SETREDRAW = 11;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

		Hash<string, IDEColor> colors;
		RichTextBox textBox;

		public ColorParser(RichTextBox textBox)
		{
			colors = Parser.IDEColors;
			this.textBox = textBox;
		}

		public void Colorize()
		{
			var position = textBox.SelectionStart;
			var length = textBox.SelectionLength;
			var normalFont = textBox.Font;
			using (var boldFont = new Font(textBox.Font, FontStyle.Bold))
			using (var italicFont = new Font(textBox.Font, FontStyle.Italic))
			{
			   StopTextBoxUpdate(textBox);

			   foreach (var color in colors.Select(item => item.Value))
			   {
			      textBox.Select(color.Position, color.Length);
			      textBox.SelectionColor = getForeColor(color);
			      textBox.SelectionBackColor = getBackColor(color);
			      Font selectionFont;
			      if (isItalic(color.Type))
			         selectionFont = italicFont;
			      else if (isBold(color.Type))
			         selectionFont = boldFont;
			      else
			         selectionFont = normalFont;
			      textBox.SelectionFont = selectionFont;
			   }

			   textBox.SelectionStart = position;
			   textBox.SelectionLength = length;

			   ResumeTextBoxUpdate(textBox);
			   textBox.Refresh();
			}
		}

		static bool isBold(IDEColor.EntityType type)
		{
			switch (type)
			{
				case IDEColor.EntityType.Structures:
				case IDEColor.EntityType.Messaging:
				case IDEColor.EntityType.KeyWords:
					return true;
				default:
					return false;
			}
		}

		static bool isItalic(IDEColor.EntityType type)
		{
			switch (type)
			{
				case IDEColor.EntityType.Variables:
					return true;
				default:
					return false;
			}
		}

		static Color getBackColor(IDEColor color)
		{
			switch (color.Type)
			{
				default:
					return Color.White;
			}
		}

		static Color getForeColor(IDEColor color)
		{
			switch (color.Type)
			{
				case IDEColor.EntityType.Strings:
					return Color.FromArgb(38, 205, 0);
				case IDEColor.EntityType.Numbers:
					return Color.DeepSkyBlue;
				case IDEColor.EntityType.Operators:
					return Color.Red;
				case IDEColor.EntityType.Variables:
					return Color.Blue;
				case IDEColor.EntityType.Structures:
					return Color.Black;
				case IDEColor.EntityType.Whitespaces:
					return Color.Black;
				case IDEColor.EntityType.Comments:
					return Color.FromArgb(128, 128, 128);
				case IDEColor.EntityType.Messaging:
					return Color.Teal;
				case IDEColor.EntityType.Formats:
					return Color.Violet;
				case IDEColor.EntityType.Dates:
					return Color.BlueViolet;
				case IDEColor.EntityType.Arrays:
					return Color.LightSalmon;
				case IDEColor.EntityType.Alternators:
					return Color.CadetBlue;
				case IDEColor.EntityType.Symbols:
					return Color.CornflowerBlue;
				case IDEColor.EntityType.Booleans:
					return Color.Coral;
				case IDEColor.EntityType.KeyWords:
					return Color.Tomato;
				case IDEColor.EntityType.Invokeables:
					return Color.CadetBlue;
				default:
					return Color.Black;
			}
		}
	}
}