using System;
using Orange.Library.Managers;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public class Variable : Value
	{
		public enum VariableAccessType
		{
			User,
			System,
			ReadOnly
		}

		protected bool isBlock;

		public Variable(string name)
		{
			Name = name;
			VariableType = VariableAccessType.User;
			isBlock = false;
		}

		public Variable()
			: this("unknown")
		{
		}

		public string Name
		{
			get;
		}

		public virtual Value Value
		{
			get => Regions[Name];
		   set
			{
				RejectNull(value, "Variable", $"Value intended for {Name} unknown");
				Regions[Name] = value;
				isBlock = value.Type == ValueType.Block;
			}
		}

		public override string ToString() => Name;

	   public override int Compare(Value value) => Value.Compare(value);

	   public override string Text
		{
			get => Value.Text;
	      set => Value.Text = value;
	   }

		public override double Number
		{
			get => Value.Number;
		   set => Value.Number = value;
		}

		public override ValueType Type
		{
			get
			{
				if (State == null)
					return ValueType.Nil;
				if (Regions.VariableExists(Name))
					return Value.Type;
				return ValueType.Nil;
			}
		}

		public override bool IsTrue => Value.IsTrue;

	   public override Value AssignmentValue() => Value;

	   public override Value Resolve() => Value;

	   public override Value Clone() => new Variable(Name);

	   public override bool IsVariable => true;

	   protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "drop", v => ((Variable)v).Delete());
			manager.RegisterMessage(this, "max-of", v => ((Variable)v).Max());
			manager.RegisterMessage(this, "min-of", v => ((Variable)v).Min());
			manager.RegisterMessage(this, "var", v => ((Variable)v).Local());
			manager.RegisterMessage(this, "default-to", v => ((Variable)v).DefaultTo());
			manager.RegisterMessage(this, "dup", v => ((Variable)v).Value.Clone());
			manager.RegisterMessage(this, "break-on", v => ((Variable)v).BreakOn());
			manager.RegisterMessage(this, "exists", v => ((Variable)v).Exists());
			Value.RegisterMessages();
		}

		public Value Exists() => Regions.VariableExists(Name);

	   public Value BreakOn()
		{
			var value = Value;
			var compare = Arguments[0];
			if (value.IsEmpty)
			{
				Value = compare;
				return false;
			}
			if (value.Compare(compare) == 0)
				return false;
			Value = compare;
			return true;
		}

		public Value For()
		{
			var block = Arguments.Block;
			if (block.CanExecute)
			{
				var test = (Block)Arguments[0];
				var increment = (Block)Arguments[1];
				if (Value.IsEmpty)
					Value = 0;
				for (; test.IsTrue && Value.Number < MAX_LOOP; increment.Evaluate())
					block.Evaluate();
			}
			return this;
		}

		public virtual Value DefaultTo()
		{
			var value = Arguments[0];
			if (Regions.VariableExists(Name) && !Value.IsEmpty)
				return Value;
			Value = value;
			return value;
		}

		public Value Local()
		{
			Regions.SetLocal(Name, "");
			return this;
		}

		public Value Max()
		{
			var value = Value;
			var compare = Arguments[0];
			if (value.IsEmpty)
			{
				Value = compare.Type == ValueType.Number ? compare.Number : compare;
				return Value;
			}
			if (compare.Type == ValueType.Number)
			{
				if (compare.Number > value.Number)
					Value = compare.Number;
			}
			else
			{
				if (string.Compare(compare.Text, value.Text, StringComparison.Ordinal) > 0)
					Value = compare;
			}
			return Value;
		}

		public Value Min()
		{
			var value = Value;
			var compare = Arguments[0];
			if (value.IsEmpty)
			{
				Value = compare.Type == ValueType.Number ? compare.Number : compare;
				return Value;
			}
			if (compare.Type == ValueType.Number)
			{
				if (compare.Number < value.Number)
					Value = compare.Number;
			}
			else
			{
				if (string.Compare(compare.Text, value.Text, StringComparison.Ordinal) < 0)
					Value = compare;
			}
			return Value;
		}

		public Value Delete()
		{
			Regions.RemoveVariable(Name);
			return null;
		}

		public override Value AlternateValue(string message)
		{
			switch (message)
			{
				case "push":
				case "pop":
				case "unshift":
				case "shift":
					if (Value.IsEmpty)
						Value = new Array();
					break;
			}
			return Value;
		}

		public override string ContainerType => ValueType.Variable.ToString();

	   public VariableAccessType VariableType
		{
			get;
			set;
		}

		public override bool IsEmpty
		{
			get
			{
				var value = Value;
				return value == null || value.IsEmpty;
			}
		}

		public virtual Value MessageTarget(string message) => Value;

	   public override Block AsBlock => isBlock ? Value.AsBlock : null;
	}
}