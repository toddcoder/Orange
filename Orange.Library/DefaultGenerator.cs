using Orange.Library.Values;

namespace Orange.Library
{
	public class DefaultGenerator : IGenerator
	{
		Value value;

		public DefaultGenerator(Value value) => this.value = value;

	   public void Before()
		{
		}

		public Value Next(int index) => index == 0 ? (Value)new Array
		{
		   value
		} : new Nil();

	   public void End()
		{
		}
	}
}