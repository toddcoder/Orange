namespace Orange.Library.Values
{
	public class ReadOnlyArray : Array
	{
		public ReadOnlyArray(Array array)
			: base(array.Items)
		{
		}

		static void throwError()
		{
			Runtime.Throw("Read only array", "Array is read-only");
		}

		public override void Add(Value value)
		{
			throwError();
		}

		public override Value AddUnique()
		{
			throwError();
			return null;
		}

		public override void AddUnique(Value value)
		{
			throwError();
		}

		public override Value Insert()
		{
			throwError();
			return null;
		}

		public override void Insert(int index, Value value)
		{
			throwError();
		}

		public override void Insert(string key, Value value)
		{
			throwError();
		}

		public override Value Merge()
		{
			throwError();
			return null;
		}

		public override Value Pop()
		{
			throwError();
			return null;
		}

		public override Value Push()
		{
			throwError();
			return null;
		}

		public override Value Remove()
		{
			throwError();
			return null;
		}

		public override void Remove(int index)
		{
			throwError();
		}

		public override void Remove(string key)
		{
			throwError();
		}

		public override Value SelfMap()
		{
			throwError();
			return null;
		}

		public override Value Shift()
		{
			throwError();
			return null;
		}

		public override Value ShiftUntil()
		{
			throwError();
			return null;
		}

		public override Value this[int index]
		{
			get => base[index];
		   set
			{
				throwError();
				base[index] = value;
			}
		}

		public override Value this[int[] keys]
		{
			get => base[keys];
		   set
			{
				throwError();
				base[keys] = value;
			}
		}

		public override Value this[string key]
		{
			get => base[key];
		   set
			{
				throwError();
				base[key] = value;
			}
		}

		public override Value this[string[] keys]
		{
			get => base[keys];
		   set
			{
				throwError();
				base[keys] = value;
			}
		}

		public override Value Unshift()
		{
			throwError();
			return null;
		}

		public override Value Clear()
		{
			throwError();
			return null;
		}

		public override string ToString() => "^" + base.ToString();
	}
}