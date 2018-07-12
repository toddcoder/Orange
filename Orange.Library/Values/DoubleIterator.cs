using System;
using System.Collections;
using System.Collections.Generic;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class DoubleIterator : Value, IIterator, IEnumerable<Value>
	{
		double start;
		double stop;
		double increment;

		public DoubleIterator(double start, double stop)
		{
			this.start = start;
			this.stop = stop;
			increment = 1;
		}

		public override int Compare(Value value)
		{
			return 0;
		}

		public override string Text
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override ValueType Type
		{
			get
			{
				return ValueType.Iterator;
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
			return new DoubleIterator(start, stop)
			{
				increment = increment
			};
		}

		protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "for", v => ((DoubleIterator)v).For());
			manager.RegisterMessageCall("apply");
			manager.RegisterMessage(this, "apply", v => ((IntIterator)v).Apply());
		}

		public Value For()
		{
			using (var assistant = new ParameterAssistant(Arguments))
			{
				var block = assistant.Block();
				if (block != null)
				{
					assistant.IteratorParameter();
					var count = 0;
					if (start < stop)
						for (var i = start; i <= stop && count++ < Runtime.MAX_LOOP; i += increment)
						{
							assistant.SetIteratorParameter(i);
							block.Evaluate();
							var signal = ParameterAssistant.Signal();
							if (signal == ParameterAssistant.SignalType.Breaking)
								break;
							switch (signal)
							{
								case ParameterAssistant.SignalType.Continuing:
									continue;
								case ParameterAssistant.SignalType.ReturningNull:
									return null;
							}
						}
					else
						for (var i = start; i >= stop && count++ < Runtime.MAX_LOOP; i -= increment)
						{
							assistant.SetIteratorParameter(i);
							block.Evaluate();
							var signal = ParameterAssistant.Signal();
							if (signal == ParameterAssistant.SignalType.Breaking)
								break;
							switch (signal)
							{
								case ParameterAssistant.SignalType.Continuing:
									continue;
								case ParameterAssistant.SignalType.ReturningNull:
									return null;
							}
						}
				}
				return this;
			}
		}

		public Value Increment
		{
			get
			{
				return increment;
			}
			set
			{
				increment = Math.Abs(value.Number);
			}
		}

		public void Iterate(Lambda lambda)
		{
			if (lambda == null)
				return;
			using (var assistant = new ParameterAssistant(Arguments))
			{
				var block = lambda.Block;
				if (block == null)
					return;

				assistant.IteratorParameter(lambda.Parameters);
				var count = 0;
				if (start < stop)
					for (var i = start; i <= stop && count++ < Runtime.MAX_LOOP; i += increment)
					{
						assistant.SetIteratorParameter(i);
						var text = block.Evaluate().Text;
						var signal = ParameterAssistant.Signal();
						if (signal == ParameterAssistant.SignalType.Breaking)
							break;
						switch (signal)
						{
							case ParameterAssistant.SignalType.Continuing:
								continue;
							case ParameterAssistant.SignalType.ReturningNull:
								return;
						}
						Runtime.State.Print(text);
					}
				else
					for (var i = start; i >= stop && count++ < Runtime.MAX_LOOP; i -= increment)
					{
						assistant.SetIteratorParameter(i);
						var text = block.Evaluate().Text;
						var signal = ParameterAssistant.Signal();
						if (signal == ParameterAssistant.SignalType.Breaking)
							break;
						switch (signal)
						{
							case ParameterAssistant.SignalType.Continuing:
								continue;
							case ParameterAssistant.SignalType.ReturningNull:
								return;
						}
						Runtime.State.Print(text);
					}
			}
		}

		public Value Apply()
		{
			var applyValue = Arguments.ApplyValue;
			var closure = applyValue as Lambda;
			if (closure != null)
				Iterate(closure);
			return this;
		}

		public IEnumerator<Value> GetEnumerator()
		{
			var count = 0;
			if (start < stop)
				for (var i = start; i <= stop && count++ < Runtime.MAX_LOOP; i += increment)
					yield return i;
			else
				for (var i = start; i >= stop && count++ < Runtime.MAX_LOOP; i -= increment)
					yield return i;
		}

		public override string ToString()
		{
			return increment == 1 ? string.Format("{0}|{1}", start, stop) : string.Format("{0}|{1}|{2}", start, stop, increment);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}