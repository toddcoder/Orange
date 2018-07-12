using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Types.Enumerables;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library
{
	public class Padder
	{
		public class MetaData
		{
			public int MaxLength
			{
				get;
				set;
			}

			public PadType PadType
			{
				get;
				set;
			}

			public void Evaluate(string value)
			{
				var length = value.Length;
				if (length > MaxLength)
					MaxLength = length;
			}
		}

		public class Row
		{
			List<string> row;

			public Row() => row = new List<string>();

		   public void AddText(string text) => row.Add(text);

		   public string this[int index]
			{
				get => row[index];
		      set => row[index] = value;
		   }

		   public bool IndexExists(int index) => index.Between(0).Until(row.Count);
		}

		List<MetaData> metaData;
		List<Row> rows;
		int rowIndex;

		public Padder(Array array)
		{
			metaData = new List<MetaData>();
			rows = new List<Row>();

			for (var i = 0; i < array.Length; i++)
				metaData.Add(new MetaData
				{
					MaxLength = 0,
					PadType = getPadType(array[i].Text)
				});

			FieldSeparator = " ";
			RecordSeparator = "\r\n";
			Trim = false;
			rowIndex = -1;
		}

		static PadType getPadType(string value)
		{
			switch (value.ToLower())
			{
				case "lpad":
				case "rjust":
					return PadType.Left;
				case "rpad":
				case "ljust":
					return PadType.Right;
				case "center":
					return PadType.Center;
				default:
					return PadType.Left;
			}
		}

		public void Evaluate(Array array)
		{
			var length = Math.Min(metaData.Count, array.Length);
			rows.Add(new Row());
			rowIndex = rows.Count - 1;
			for (var i = 0; i < length; i++)
			{
				var text = array[i].Text;
				rows[rowIndex].AddText(text);
				var data = metaData[i];
				data.Evaluate(text);
			}
		}

		public void Evaluate(params string[] values)
		{
			var length = Math.Min(metaData.Count, values.Length);
			rows.Add(new Row());
			rowIndex = rows.Count - 1;
			for (var i = 0; i < length; i++)
			{
				var text = values[i];
				rows[rowIndex].AddText(text);
				var data = metaData[i];
				data.Evaluate(text);
			}
		}

		public string EvaluateString(string text)
		{
			var records = State.RecordPattern.Split(text);
			foreach (var fields in records.Select(record => State.FieldPattern.Split(record)))
				Evaluate(fields);
			return ToString();
		}

		public string RecordSeparator
		{
			get;
			set;
		}

		public string FieldSeparator
		{
			get;
			set;
		}

		public bool Trim
		{
			get;
			set;
		}

		public override string ToString() => records().Listify(RecordSeparator);

	   string[] records()
		{
			var list = new List<string>();
			foreach (var row in rows)
			{
				var fields = new List<string>();
				for (var j = 0; j < metaData.Count; j++)
				{
					var data = metaData[j];
					var value = row.IndexExists(j) ? row[j] : "";
					if (value == null)
						continue;
					var paddedItem = value.IsMatch("^ ['=^*-'] $") ? value.Repeat(data.MaxLength) :
                  value.Pad(data.PadType, data.MaxLength);
					fields.Add(paddedItem);
				}
				var rowText = fields.Listify(FieldSeparator);
				if (Trim)
					rowText = rowText.Trim();
				list.Add(rowText);
			}
			return list.ToArray();
		}

		public Array Array
		{
			get
			{
				var array = new Array();
				foreach (var data in metaData)
					array.Add(data.PadType.ToString().ToLower());
				return array;
			}
		}

		public Array GetArray() => new Array(records());

	   public int Length => metaData.Sum(d => d.MaxLength) + (metaData.Count - 1) * State.FieldSeparator.Text.Length;
	}
}