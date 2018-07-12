using System;
using System.Drawing;
using System.Windows.Forms;
using Orange.Library.Debugging;
using Standard.Types.Collections;

namespace Orange.IDE
{
	public class BreakpointSave
	{
		class ColorSaver
		{
			Func<RichTextBox, Color> colorPicker;
			Action<RichTextBox, Color> colorSaver;
			Hash<Position, Color> colors;
			Color? currentColor;
			Position position;

			public ColorSaver(Func<RichTextBox, Color> colorPicker, Action<RichTextBox, Color> colorSaver, Position position)
			{
				this.colorPicker = colorPicker;
				this.colorSaver = colorSaver;
				this.position = position;
				this.position.Length = 0;
				colors = new Hash<Position, Color>();
				currentColor = null;
			}

			public void Save(RichTextBox textBox)
			{
				var selectedColor = colorPicker(textBox);
				if (currentColor == null || currentColor.Value != selectedColor)
				{
					var selectedPosition = new Position(position.StartIndex, position.Length);
					position.StartIndex += position.Length;
					position.Length = 0;
					colors[selectedPosition] = selectedColor;
					currentColor = selectedColor;
				}
				else
					position.Length++;
			}

			public void Restore(RichTextBox textBox)
			{
				foreach (var item in colors)
				{
					var currentPosition = item.Key;
					textBox.Select(currentPosition.StartIndex, currentPosition.Length);
					colorSaver(textBox, item.Value);
				}
			}
		}

		Position position;
		ColorSaver colorSaver;
		ColorSaver backColorSaver;
		Position oldPosition;

		public BreakpointSave(Position position)
		{
			this.position = position;
			colorSaver = new ColorSaver(rtb => rtb.SelectionColor, (rtb, c) => rtb.SelectionColor = c, this.position);
			backColorSaver = new ColorSaver(rtb => rtb.SelectionBackColor, (rtb, c) => rtb.SelectionBackColor = c, this.position);
		}

		public void Begin(RichTextBox textBox)
		{
			oldPosition = new Position(textBox.SelectionStart, textBox.SelectionLength);
			ColorParser.StopTextBoxUpdate(textBox);
		}

		public void End(RichTextBox textBox)
		{
			textBox.Select(oldPosition.StartIndex, oldPosition.Length);
			ColorParser.ResumeTextBoxUpdate(textBox);
			textBox.Refresh();
		}

		public void Save(RichTextBox textBox)
		{
			for (var i = 0; i < position.Length; i++)
			{
				var index = position.StartIndex + i;
				textBox.Select(index, 1);
				colorSaver.Save(textBox);
				backColorSaver.Save(textBox);
			}
		}

		public void Restore(RichTextBox textBox)
		{
			ColorParser.StopTextBoxUpdate(textBox);
			colorSaver.Restore(textBox);
			backColorSaver.Restore(textBox);
		}
	}
}