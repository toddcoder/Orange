using System.Collections.Generic;
using Orange.Library.Managers;
using Standard.Types.Collections;

namespace Orange.Library.Values
{
	public class OldCase : Value
	{
		public static int Match(Value left, Value right, Namespace ns, bool usePopper = true)
		{
			using (var popper = new NamespacePopper(ns, "case match"))
			{
				popper.Push();
				Runtime.State.SetLocal(Runtime.State.DefaultParameterNames.ValueVariable, left);
				if (left.ID == right.ID)
					return 0;
				if (right.Type == ValueType.Any)
					return 0;
				if (right.Type == ValueType.Placeholder)
				{
					Runtime.State.SetLocal(right.Text, left);
					return 0;
				}
				if (right.IsArray && left.IsArray)
				{
					var leftArray = (Array)left.SourceArray;
					var rightArray = (Array)right.SourceArray;
					int match = leftArray.Match(rightArray);
					return match;
				}

				var obj1 = left as Object;
				if (obj1 != null)
				{
					var obj2 = right as Object;
					if (obj2 != null)
					{
						var chains = new Hash<string, MessageChain>();
						var bindings = new Hash<string, Value>
						{
							DefaultValue = ""
						};
						var repeating = false;
						int comparison;
						while (true)
						{
							comparison = obj1.Compare(obj2, chains, new MessageChain(), bindings, ref repeating);
							if (comparison == 0)
							{
								foreach (KeyValuePair<string, MessageChain> item in chains)
									Runtime.State[item.Key, true] = item.Value.Invoke(obj1);
								return comparison;
							}
							if (!repeating)
								break;
						}
						return comparison;
					}
					var cls = right as Class;
					if (cls != null)
						return obj1.Class.ClassName.CompareTo(cls.ClassName);
				}
				var pattern = right as Pattern;
				if (pattern != null)
					return pattern.IsMatch(left.Text) ? 0 : 1;
				var obj = right as Object;
				if (obj != null)
				{
					Class cls = obj.Class;
					Value value = CompareClass(cls, left);
					if (value == null || value.Type == ValueType.Nil)
						return 1;
					Runtime.Assert(value.Type == ValueType.Object, "Case", "extract must return an Object or a nil");
					return Match(value, obj, ns, false);
				}
				return left.Compare(right);
			}
		}

		public static Value CompareClass(Class cls, Value other)
		{
			return Runtime.State.SendMessage(cls, cls.RespondsTo("parse") ? "parse" : "insp", other);
		}

		Value value;
		Value result;

		public OldCase(Value value, Value result)
		{
			this.value = value;
			this.result = result;
		}

		public OldCase(OldCase _case, Value result)
		{
			value = _case.value;
			this.result = result;
		}

		public override int Compare(Value value)
		{
			return Match(this.value, value, new Namespace());
		}

		public int Match(Value value, Namespace ns)
		{
			return Match(this.value, value, ns);
		}

		public override string Text
		{
			get
			{
				return value.Text;
			}
			set
			{
			}
		}

		public override double Number
		{
			get
			{
				return value.Number;
			}
			set
			{
			}
		}

		public override ValueType Type
		{
			get
			{
				return ValueType.Case;
			}
		}

		public override bool IsTrue
		{
			get
			{
				return !result.IsNil;
			}
		}

		public override Value Clone()
		{
			return new OldCase(value.Clone(), result);
		}

		protected override void registerMessages(MessageManager manager)
		{
		}

		public override string ToString()
		{
			return value + ".case";
		}

		public override Value AlternateValue(string message)
		{
			return result.IsNil ? new Nil() : result;
		}

		public override Value ArgumentValue()
		{
			return AlternateValue("");
		}

		public override Value AssignmentValue()
		{
			return AlternateValue("");
		}

		public Value Value
		{
			get
			{
				return value;
			}
		}

		public Value Result
		{
			get
			{
				return result;
			}
		}
	}
}