using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class Enumerator : Value
	{
		Object obj;

		public Enumerator(Object obj)
		{
			this.obj = obj;
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
				return ValueType.Enumerator;
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
			return new Enumerator(obj);
		}

		protected override void registerMessages(MessageManager manager)
		{
		}

		public override bool IsArray
		{
			get
			{
				return true;
			}
		}

		public override Value SourceArray
		{
			get
			{
				if (obj.RespondsTo("array"))
					return Runtime.State.SendMessage(obj, "array");
				var array = new Array();
				Runtime.State.SendMessage(obj, "reset");
				var index = 0;
				while (index++ <= Runtime.MAX_LOOP)
				{
					Value value = Runtime.State.SendMessage(obj, "next");
					if (value.Type == ValueType.Nil)
						break;
					array.Add(value);
				}
				return array;
			}
		}

		public override string ToString()
		{
			return obj.ToString();
		}
	}
}