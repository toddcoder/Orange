using Orange.Library.Managers;

namespace Orange.Library.Values
{
	public class JoinedArrays : Value
	{
		Array left;
		Array right;

		public JoinedArrays(Array left, Array right)
		{
			this.left = left;
			this.right = right;
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
				return ValueType.JoinedArray;
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
			return new JoinedArrays((Array)left.Clone(), (Array)right.Clone());
		}

		protected override void registerMessages(MessageManager manager)
		{
			manager.RegisterMessage(this, "if", v => ((JoinedArrays)v).If());
			manager.RegisterMessage(this, "unless", v => ((JoinedArrays)v).Unless());
			manager.RegisterMessage(this, "map", v => ((JoinedArrays)v).Map());
		}

		public override Value AlternateValue(string message)
		{
			return SourceArray;
		}

		public Value If()
		{
			Block block = Arguments.Executable;
			if (block.CanExecute)
			{
				string key1Var;
				string value1Var;
				string index1Var;
				string key2Var;
				string value2Var;
				string index2Var;
				Arguments.Parameters.ArrayParameters(out key1Var, out value1Var, out index1Var, out key2Var, out value2Var, out index2Var);
				var leftResult = new Array();
				var rightResult = new Array();
				block.ReturnNull = false;
				foreach (Array.IterItem outer in left)
				{
					Runtime.State.SetLocal(key1Var, outer.Key);
					Runtime.State.SetLocal(value1Var, outer.Value);
					Runtime.State.SetLocal(index1Var, outer.Index);
					foreach (Array.IterItem inner in right)
					{
						Runtime.State.SetLocal(key2Var, inner.Key);
						Runtime.State.SetLocal(value2Var, inner.Value);
						Runtime.State.SetLocal(index2Var, inner.Index);
						if (block.Evaluate().IsTrue)
						{
							leftResult[outer.Key] = outer.Value;
							rightResult[inner.Key] = inner.Value;
						}
					}
				}
				return new JoinedArrays(leftResult, rightResult);
			}
			return null;
		}

		public Value Unless()
		{
			Block block = Arguments.Executable;
			if (block.CanExecute)
			{
				string key1Var;
				string value1Var;
				string index1Var;
				string key2Var;
				string value2Var;
				string index2Var;
				Arguments.Parameters.ArrayParameters(out key1Var, out value1Var, out index1Var, out key2Var, out value2Var, out index2Var);
				var leftResult = new Array();
				var rightResult = new Array();
				block.ReturnNull = false;
				foreach (Array.IterItem outer in left)
				{
					Runtime.State.SetLocal(key1Var, outer.Key);
					Runtime.State.SetLocal(value1Var, outer.Value);
					Runtime.State.SetLocal(index1Var, outer.Index);
					foreach (Array.IterItem inner in right)
					{
						Runtime.State.SetLocal(key2Var, inner.Key);
						Runtime.State.SetLocal(value2Var, inner.Value);
						Runtime.State.SetLocal(index2Var, inner.Index);
						if (!block.Evaluate().IsTrue)
						{
							leftResult[outer.Key] = outer.Value;
							rightResult[inner.Key] = inner.Value;
						}
					}
				}
				return new JoinedArrays(leftResult, rightResult);
			}
			return null;
		}

		public Value Map()
		{
			Block block = Arguments.Executable;
			if (block.CanExecute)
			{
				string key1Var;
				string value1Var;
				string index1Var;
				string key2Var;
				string value2Var;
				string index2Var;
				Arguments.Parameters.ArrayParameters(out key1Var, out value1Var, out index1Var, out key2Var, out value2Var, out index2Var);
				var result = new Array();
				block.ReturnNull = true;
				foreach (Array.IterItem outer in left)
				{
					Runtime.State.SetLocal(key1Var, outer.Key);
					Runtime.State.SetLocal(value1Var, outer.Value);
					Runtime.State.SetLocal(index1Var, outer.Index);
					foreach (Array.IterItem inner in right)
					{
						Runtime.State.SetLocal(key2Var, inner.Key);
						Runtime.State.SetLocal(value2Var, inner.Value);
						Runtime.State.SetLocal(index2Var, inner.Index);
						Value value = block.Evaluate();
						if (value != null)
							result.Add(value);
					}
				}
				return result;
			}
			return null;
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
				var array = new Array();
				foreach (Array.IterItem outer in left)
				{
					foreach (Array.IterItem inner in right)
					{
						var tuple = new Tuple(outer.Value, inner.Value);
						array.Add(tuple);
					}
				}
				return array;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} <> {1}", left, right);
		}
	}
}