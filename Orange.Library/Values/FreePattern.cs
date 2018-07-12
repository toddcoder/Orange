using System.Collections.Generic;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class FreePattern : Value
	{
		class State
		{
			public int Position
			{
				get;
				set;
			}
		}

		const string LOC_FREE_PATTERN = "Free pattern";

		string subject;
		Stack<State> stateStack;

		public FreePattern(string subject)
		{
			this.subject = subject;
			stateStack = new Stack<State>();
			stateStack.Push(new State());
		}

		public override int Compare(Value value)
		{
			return 0;
		}

		public override string Text
		{
			get;
			set;
		}

		public override double Number
		{
			get;
			set;
		}

		public override ValueType Type
		{
			get
			{
				return ValueType.FreePattern;
			}
		}

		public override bool IsTrue
		{
			get
			{
				return false;
			}
		}

		public override Value Clone()
		{
			return null;
		}

		protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "up-to", v => ((FreePattern)v).UpTo());
			manager.RegisterMessage(this, "many", v => ((FreePattern)v).Many());
			manager.RegisterMessage(this, "any", v => ((FreePattern)v).Any());
			manager.RegisterMessage(this, "bal", v => ((FreePattern)v).Balance());
			manager.RegisterMessage(this, "find", v => ((FreePattern)v).Find());
			manager.RegisterMessage(this, "match", v => ((FreePattern)v).Match());
		}

		public Value UpTo()
		{
			return this;
		}

		public Value Many()
		{
			return this;
		}

		public Value Any()
		{
			return this;
		}

		public Value Balance()
		{
			return this;
		}

		public Value Find()
		{
			return this;
		}

		public Value Match()
		{
			return this;
		}
	}
}