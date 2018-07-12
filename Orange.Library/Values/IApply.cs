namespace Orange.Library.Values
{
	public interface IApply
	{
		Value Apply(Value argument);
		Value ApplyWhileTrue(Value argument);
	}
}