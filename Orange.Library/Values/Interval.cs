using System;
using Orange.Library.Managers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class Interval : Value
	{
		const string LOCATION = "Interval";

		public static implicit operator Interval(string text)
		{
			var matcher = new Matcher();
			string hours;
			string minutes;
			string seconds;
			string milliseconds;
			if (matcher.IsMatch(text, "^ /(/d+) /s* '.' /s* /(/d+) /s* ':' /s* /(/d+) /s* ':' /s* /(/d+) /s* '.' /s* /(/d+) $"))
			{
				string days;
				matcher.Extract(0, out days, out hours, out minutes, out seconds, out milliseconds);
				return new Interval(new TimeSpan(days.ToInt(), hours.ToInt(), minutes.ToInt(), seconds.ToInt(),
               milliseconds.ToInt()));
			}
			if (matcher.IsMatch(text, "^ /(/d+) /s* ':' /s* /(/d+) /s* ':' /s* /(/d+) /s* '.' /s* /(/d+) $"))
			{
				matcher.Extract(0, out hours, out minutes, out seconds, out milliseconds);
				return new Interval(new TimeSpan(0, hours.ToInt(), minutes.ToInt(), seconds.ToInt(), milliseconds.ToInt()));
			}
			if (matcher.IsMatch(text, "^ ':' /s* /(/d+) /s* ':' /s* /(/d+) /s* '.' /s* /(/d+) $"))
			{
				matcher.Extract(0, out minutes, out seconds, out milliseconds);
				return new Interval(new TimeSpan(0, 0, minutes.ToInt(), seconds.ToInt(), milliseconds.ToInt()));
			}
			if (matcher.IsMatch(text, "^ ':' /s* /(/d+) /s* '.' /s* /(/d+) $"))
			{
				matcher.Extract(0, out seconds, out milliseconds);
				return new Interval(new TimeSpan(0, 0, 0, seconds.ToInt(), milliseconds.ToInt()));
			}
			if (matcher.IsMatch(text, "^ /(/d+) $"))
			{
				matcher.Extract(0, out milliseconds);
				return new Interval(new TimeSpan(0, 0, 0, 0, milliseconds.ToInt()));
			}
			Throw(LOCATION, $"Didn't understand '{text}'");
			return null;
		}

		TimeSpan timeSpan;

		public Interval(TimeSpan timeSpan) => this.timeSpan = timeSpan;

	   public override int Compare(Value value) => Number.CompareTo(value.Number);

	   public override string Text
		{
			get
			{
				return timeSpan.ToString();
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return timeSpan.Ticks;
			}
			set
			{
			}
		}

		public override ValueType Type => ValueType.Interval;

	   public override bool IsTrue => true;

	   public override Value Clone() => new Interval(timeSpan);

	   protected override void registerMessages(MessageManager manager)
		{
		}

		public TimeSpan TimeSpan => timeSpan;
	}
}