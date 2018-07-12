using System.Text;
using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library
{
	public class Buffer
	{
		StringBuilder builder;
		bool putting;

		public Buffer()
		{
			builder = new StringBuilder();
			putting = false;
		}

		public void Print(string text)
		{
			if (builder.Length != 0)
				builder.Append(State.RecordSeparator.Text);
			builder.Append(text);
			putting = false;
		}

		public void Print(string format, params object[] args) => Print(string.Format(format, args));

	   public void Print(Value value) => Print(value.ToString());

	   public void Put(string text)
		{
			if (putting)
				builder.Append(State.FieldSeparator.Text);
			builder.Append(text);
			putting = true;
		}

		public void Write(string text) => builder.Append(text);

	   public string Result()
		{
			var result = builder.ToString();
			Clear();
			return result;
		}

		public override string ToString() => builder.ToString();

	   public void Clear()
		{
			builder.Clear();
			putting = false;
		}
	}
}