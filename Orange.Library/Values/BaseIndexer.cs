using static Orange.Library.Compiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
	public abstract class BaseIndexer<T> : Variable
	{
		protected Array array;

		protected BaseIndexer(Array array)
			: base(VAR_ANONYMOUS + CompilerState.ObjectID()) => this.array = array;

	   protected abstract T[] getIndicators();

		protected abstract Value getSlice(T[] indicators);

		protected abstract void setSlice(T[] indicators, Value value);

		public override Value Value
		{
			get => getSlice(getIndicators());
		   set => setSlice(getIndicators(), value);
		}

		public override Value AlternateValue(string message) => Value;

	   protected abstract void setLength();

		public abstract Value SelfMap();

		public abstract Value Remove();

		public abstract Value Fill();

		public abstract Value Insert();

		public abstract Value Swap();

		public override string ContainerType => ValueType.Indexer.ToString();

	   public override Value MessageTarget(string message) => this;

	   public override bool IsIndexer => true;

	   public override Value Resolve() => Value;

	   public override string ToString() => Value.ToString();

	   public abstract Value Get();
	}
}