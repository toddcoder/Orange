using Orange.Library.Values;

namespace Orange.Library
{
	public enum TailCallSearchType
	{
		Cancel,
		TailCallVerb,
		PushVariable,
		NameProperty,
		NestedBlock
	}

	public interface ITailCallVerb
	{
		TailCallSearchType TailCallSearchType
		{
			get;
		}

		string NameProperty
		{
			get;
		}

		Block NestedBlock
		{
			get;
		}
	}
}