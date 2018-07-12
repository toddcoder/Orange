using System.Collections.Generic;
using Orange.Library.Managers;
using System.Text;
using Standard.Types.Enumerables;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class Cube : Value
	{
		List<string> layers;
		int lastLayer;

		public Cube(string layer)
		{
			layers = new List<string>
			{
				layer
			};
			lastLayer = 0;
		}

	   public Cube(List<string> layers, int lastLayer)
	   {
	      this.layers = layers;
	      this.lastLayer = lastLayer;
	   }

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get
			{
				var result = layers[0];
				var pattern = State.RecordPattern;
				var connector = State.RecordSeparator.Text;
				for (var i = 1; i < layers.Count; i++)
				{
					var sourceRows = pattern.Split(layers[i]);
					var targetRows = pattern.Split(result);
					var sourceLength = sourceRows.Length;
					var targetLength = targetRows.Length;
					if (targetLength < sourceLength)
					{
						var targetList = new List<string>(targetRows);
						for (var j = 0; j < sourceLength - targetLength; j++)
							targetList.Add("");
						targetRows = targetList.ToArray();
					}
					for (var j = 0; j < sourceLength; j++)
						targetRows[j] = evaluateRow(sourceRows[j], targetRows[j]);
					result = targetRows.Listify(connector);
				}
				return result;
			}
			set
			{
			}
		}

	   static string evaluateRow(string sourceRow, string targetRow)
		{
			var target = new StringBuilder(targetRow);
			var sourceLength = sourceRow.Length;
			var targetLength = target.Length;
			if (targetLength < sourceLength)
				target.Append(" ".Repeat(sourceLength - targetLength));
			for (var i = 0; i < sourceLength; i++)
			{
				var ch = sourceRow[i];
				if (!char.IsWhiteSpace(ch))
					target[i] = ch;
			}
			return target.ToString();
		}

		public override double Number
		{
			get
			{
				return Text.ToDouble();
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Cube;

	   public override bool IsTrue => false;

	   public override Value Clone() => new Cube(layers, lastLayer);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "addLayer", v => ((Cube)v).Layer());
			manager.RegisterMessage(this, "at", v => ((Cube)v).At());
			manager.RegisterMessage(this, "addRow", v => ((Cube)v).AddRow());
		}

		public override Value AlternateValue(string message) => Text;

	   public Value AddRow()
		{
			var text = Arguments[0].Text;
			var count = (int)Arguments[1].Number;
			if (count > 0)
				text = text.Repeat(count);
			var target = layers[lastLayer];
			target += State.RecordSeparator.Text + text;
			layers[lastLayer] = target;
			return this;
		}

		public Value At()
		{
			var row = (int)Arguments[0].Number;
			var col = (int)Arguments[1].Number;
			var text = Arguments[2].Text;
			var layer = layers[lastLayer];
			layer = position(row, col, text, layer);
			layers[lastLayer] = layer;
			return this;
		}

		string position(int row, int column, string text, string layer)
		{
			if (row < 0)
				row = 0;
			if (column < 0)
				column = 0;
			var recordPattern = State.RecordPattern;
			var rowsToAdd = new List<string>();
			var sourceRows = recordPattern.Split(text);
			var targetRows = recordPattern.Split(layer);
			for (var i = 0; i < sourceRows.Length; i++)
			{
				var targetRow = i + row;
				var sourceRow = sourceRows[i];
				var sourceLength = sourceRow.Length;
				var paddingLength = sourceLength + column;
				var add = targetRow > targetRows.Length;
				var target = add ? new StringBuilder(" ".Repeat(paddingLength)) :
               new StringBuilder(targetRows[targetRow].PadRight(sourceLength + column));
				for (var j = 0; j < sourceRow.Length; j++)
				{
					var targetCol = j + column;
					var ch = sourceRow[j];
					if (ch != '~')
						target[targetCol] = sourceRow[j];
				}
				if (add)
					rowsToAdd.Add(target.ToString());
				else
					targetRows[targetRow] = target.ToString();
			}
			var connector = State.RecordSeparator.Text;
			var result = targetRows.Listify(connector);
			if (rowsToAdd.Count > 0)
			{
				var toAppend = rowsToAdd.Listify(connector);
				if (result.IsNotEmpty())
					result += connector + toAppend;
				else
					result = toAppend;
			}
			return result;
		}

		public Value Layer()
		{
			layers.Add("");
			lastLayer = layers.Count - 1;
			return this;
		}
	}
}