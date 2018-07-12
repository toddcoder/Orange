using Orange.Library.Values;

namespace Orange.Library
{
	public abstract class GetDefault
	{
		public static GetDefault Create(Value value) => value.IsArray ? (GetDefault)new GetDefaultArray((Array)value.SourceArray) : new GetDefaultValue(value);

	   public abstract Value Value();
	}

	public class GetDefaultValue : GetDefault
	{
		Value value;

		public GetDefaultValue(Value value) => this.value = value;

	   public override Value Value() => value;
	}

	public class GetDefaultArray : GetDefault
	{
		Array array;

		public GetDefaultArray(Array array) => this.array = array;

	   public override Value Value() => array.Shift();
	}
}