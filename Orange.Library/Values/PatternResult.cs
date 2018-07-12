using System.Text;
using Orange.Library.Managers;
using Standard.Types.Strings;

namespace Orange.Library.Values
{
	public class PatternResult : Value
	{
		public static PatternResult Falure() => new PatternResult
		{
		   Success = false,
		   StartIndex = -1,
		   StopIndex = -1,
		   Input = "",
		   Text = "",
		   Rest = "",
		   Value = ""
		};

	   public string Input
		{
			get;
			set;
		}

		public bool Success
		{
			get;
			set;
		}

		public int StartIndex
		{
			get;
			set;
		}

		public int StopIndex
		{
			get;
			set;
		}

		public int Length => StopIndex - StartIndex;

	   public string Rest
		{
			get;
			set;
		}

		public Variable Variable
		{
			get;
			set;
		}

		public override int Compare(Value value) => 0;

	   public override string Text
		{
			get;
			set;
		}

		public override double Number
		{
			get
			{
				return Length;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.PatternResult;

	   public override bool IsTrue => Success;

	   public int Position
		{
			get;
			set;
		}

		public override Value Clone() => new PatternResult
		{
		   Input = Input,
		   Text = Text,
		   Success = Success,
		   StartIndex = StartIndex,
		   StopIndex = StopIndex,
		   Value = Value.Clone(),
		   Rest = Rest,
		   Position = Position
		};

	   public Value Value
		{
			get;
			set;
		}

		protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "index", v => ((PatternResult)v).StartIndex);
			manager.RegisterMessage(this, "len", v => ((PatternResult)v).Length);
			manager.RegisterMessage(this, "success", v => ((PatternResult)v).Success);
			manager.RegisterMessage(this, "input", v => ((PatternResult)v).Input);
			manager.RegisterMessage(this, "val", v => ((PatternResult)v).Value);
			manager.RegisterMessage(this, "matched", v => ((PatternResult)v).Matched());
			manager.RegisterMessage(this, "bef", v => ((PatternResult)v).Before());
			manager.RegisterMessage(this, "aft", v => ((PatternResult)v).After());
			manager.RegisterMessage(this, "concat", v => ((PatternResult)v).Concat());
		}

		public Value Matched() => Success ? Input.Substring(StartIndex, Length) : "";

	   public Value Before() => Success ? Input.Slice(0, StartIndex - 1) : "";

	   public Value After() => Success ? Input.Skip(StopIndex) : "";

	   public Value Concat()
		{
			var value = Arguments[0];
			var text = value.Type == ValueType.PatternResult ? ((PatternResult)value).Matched().Text : value.Text;
			return Matched().Text + text;
		}

		static string visibleSpaces(string text) => text.Replace("\r", "µ").Replace("\n", "¶").Replace("\t", "¬").Replace(" ", "•");

	   public override string ToString()
		{
			if (Success)
			{
				var result = new StringBuilder("[");
				Slicer slicer = visibleSpaces(Input);
				slicer[StartIndex, Length] = "§".Repeat(Length);
				result.Append(slicer);
				result.Append("]");
				if (Input != Text)
				{
					result.Append("[");
					result.Append(visibleSpaces(Text));
					result.Append("]");
				}
				return result.ToString();
			}
			return "Failure";

		}
	}
}