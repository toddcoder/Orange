using System;
using Orange.Library.Managers;
using Standard.Types.Dates;

namespace Orange.Library.Values
{
	public class Date : Value
	{
		public enum DatePartType
		{
			Year,
			Month,
			Day,
			Hour,
			Minute,
			Second,
			Millisecond
		}

		public static implicit operator Date(DateTime dateTime) => new Date(dateTime);

	   DateTime dateTime;

		public Date(DateTime dateTime) => this.dateTime = dateTime;

	   public Date(double ticks) => dateTime = new DateTime((long)ticks);

	   public Date(Value value)
		{
			if (value.Type == ValueType.Date)
				dateTime = ((Date)value).dateTime;
		   dateTime = DateTime.TryParse(value.Text, out var converted) ? converted : new DateTime((long)value.Number);
		}

		public override int Compare(Value value) => dateTime.Ticks.CompareTo((long)value.Number);

	   public override string Text
		{
			get => dateTime.ToString();
	      set => dateTime = DateTime.Parse(value);
	   }

		public override double Number
		{
			get => dateTime.Ticks;
		   set => dateTime = new DateTime((long)value);
		}

		public override ValueType Type => ValueType.Date;

	   public override bool IsTrue => true;

	   public override Value Clone() => new Date(dateTime);

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "trunc", v => ((Date)v).Truncate());
			manager.RegisterMessage(this, "addYears", v => ((Date)v).Add(DatePartType.Year));
			manager.RegisterMessage(this, "addMonths", v => ((Date)v).Add(DatePartType.Month));
			manager.RegisterMessage(this, "addDays", v => ((Date)v).Add(DatePartType.Day));
			manager.RegisterMessage(this, "addHours", v => ((Date)v).Add(DatePartType.Hour));
			manager.RegisterMessage(this, "addMinutes", v => ((Date)v).Add(DatePartType.Minute));
			manager.RegisterMessage(this, "addSeconds", v => ((Date)v).Add(DatePartType.Second));
			manager.RegisterMessage(this, "addMilliseconds", v => ((Date)v).Add(DatePartType.Millisecond));
			manager.RegisterMessage(this, "diff", v => ((Date)v).Difference());
			manager.RegisterMessage(this, "year", v => ((Date)v).Year());
			manager.RegisterMessage(this, "month", v => ((Date)v).Month());
			manager.RegisterMessage(this, "day", v => ((Date)v).Day());
			manager.RegisterMessage(this, "hour", v => ((Date)v).Hour());
			manager.RegisterMessage(this, "minute", v => ((Date)v).Minute());
			manager.RegisterMessage(this, "second", v => ((Date)v).Second());
			manager.RegisterMessage(this, "msecond", v => ((Date)v).Millisecond());
			manager.RegisterMessage(this, "months", v => Months());
			manager.RegisterMessage(this, "dims", v => DaysInMonth());
			manager.RegisterMessage(this, "dows", v => DaysInWeek());
			manager.RegisterMessage(this, "ticks", v => ((Date)v).Ticks());
			manager.RegisterMessage(this, "dow", v => ((Date)v).DOW());
			manager.RegisterMessage(this, "utcOffset", v => ((Date)v).UTCOffset());
			manager.RegisterMessage(this, "add", v => ((Date)v).Add());
			manager.RegisterMessage(this, "sub", v => ((Date)v).Sub());
		}

		public Value Add()
		{
			Interval interval = Arguments[0].Text;
			return Add(interval);
		}

		public Date Add(Interval interval) => dateTime.Add(interval.TimeSpan);

	   public Value Sub()
		{
			Interval interval = Arguments[0].Text;
			return Sub(interval);
		}

		public DateTime Sub(Interval interval) => dateTime.Subtract(interval.TimeSpan);

	   public Value DOW() => dateTime.DayOfWeek.ToString();

	   public Value Ticks() => dateTime.Ticks;

	   public override Value AlternateValue(string message) => Text;

	   public static Value Months() => new Array(new[]
	   {
	      "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November",
	      "December"
	   });

	   public static Value DaysInMonth() => new Array(new Value[]
		{
		   31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
		});

	   public static Value DaysInWeek() => new Array(new[]
		{
		   "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
		});

	   public Value Year() => dateTime.Year;

	   public Value Month() => dateTime.Month;

	   public Value Day() => dateTime.Day;

	   public Value Hour() => dateTime.Hour;

	   public Value Minute() => dateTime.Minute;

	   public Value Second() => dateTime.Second;

	   public Value Millisecond() => dateTime.Millisecond;

	   public Value Truncate() => dateTime.Truncate();

	   public Value Add(DatePartType partType)
		{
			var amount = Arguments[0].Number;
			DateTime result;
			switch (partType)
			{
				case DatePartType.Year:
					result = dateTime.AddYears((int)amount);
					break;
				case DatePartType.Month:
					result = dateTime.AddMonths((int)amount);
					break;
				case DatePartType.Day:
					result = dateTime.AddDays(amount);
					break;
				case DatePartType.Hour:
					result = dateTime.AddHours(amount);
					break;
				case DatePartType.Minute:
					result = dateTime.AddMinutes(amount);
					break;
				case DatePartType.Second:
					result = dateTime.AddSeconds(amount);
					break;
				case DatePartType.Millisecond:
					result = dateTime.AddMilliseconds(amount);
					break;
				default:
					return Clone();
			}
			return result;
		}

		public Value Difference() => (dateTime - ((Date)Arguments[0]).dateTime).Ticks;

	   public override string ToString() => $"<#{Text}>";

	   public DateTime DateTime => dateTime;

	   public Value UTCOffset() => TimeZone.CurrentTimeZone.GetUtcOffset(dateTime).TotalHours;
	}
}