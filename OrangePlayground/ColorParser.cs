using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Orange.Library;
using Orange.Library.Parsers;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using static System.Drawing.Color;
using static System.Drawing.FontStyle;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace OrangePlayground
{
   public class ColorParser : IColorizer
   {
      public static void StopTextBoxUpdate(RichTextBox textBox) => SendMessage(textBox.Handle, WM_SETREDRAW, false, 0);

      public static void ResumeTextBoxUpdate(RichTextBox textBox) => SendMessage(textBox.Handle, WM_SETREDRAW, true, 0);

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
         using (var boldFont = new Font(textBox.Font, Bold))
         using (var italicFont = new Font(textBox.Font, Italic))
         using (var underlineFont = new Font("Monoid", textBox.Font.Size, Underline))
         using (var specialFont = new Font("Monoid", textBox.Font.Size, Regular))
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
               else if (isUnderline(color.Type))
                  selectionFont = underlineFont;
               else if (isSpecial(color.Type))
                  selectionFont = specialFont;
               else
                  selectionFont = normalFont;
               textBox.SelectionFont = selectionFont;
            }

            //markText("/t", GhostWhite);
            markText("/s+ (/r /n | /r | /n)", PaleVioletRed);

            textBox.SelectionStart = position;
            textBox.SelectionLength = length;

            ResumeTextBoxUpdate(textBox);
            textBox.Refresh();
         }
      }

      void markText(string pattern, Color backColor)
      {
         if (textBox.Text.Matches(pattern).If(out var matcher))
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var match = matcher.GetMatch(i);
               textBox.Select(match.Index, match.Length);
               textBox.SelectionBackColor = backColor;
            }
      }

      static bool isBold(EntityType type)
      {
         switch (type)
         {
            case Structures:
            case Messaging:
            case KeyWords:
               return true;
            default:
               return false;
         }
      }

      static bool isItalic(EntityType type)
      {
         switch (type)
         {
            case Variables:
               return true;
            default:
               return false;
         }
      }

      static bool isUnderline(EntityType type) => type == Types;

      static bool isSpecial(EntityType type) => type == Strings || type == Interpolated;

      static Color getBackColor(IDEColor color)
      {
         switch (color.Type)
         {
            default:
               return White;
         }
      }

      static Color getForeColor(IDEColor color)
      {
         switch (color.Type)
         {
            case Strings:
               return FromArgb(38, 205, 0);
            case Numbers:
               return DeepSkyBlue;
            case Operators:
               return Red;
            case Variables:
               return Blue;
            case Structures:
               return Black;
            case Whitespaces:
               return Black;
            case Comments:
               return FromArgb(128, 128, 128);
            case Messaging:
               return Teal;
            case Formats:
               return Violet;
            case Dates:
               return DarkOliveGreen;
            case Arrays:
               return LightSalmon;
            case Alternators:
               return CadetBlue;
            case Symbols:
               return CornflowerBlue;
            case Booleans:
               return Coral;
            case KeyWords:
               return BlueViolet;
            case Invokeables:
               return CadetBlue;
            case Interpolated:
               return DodgerBlue;
            case Types:
               return CadetBlue;
            default:
               return Black;
         }
      }
   }
}