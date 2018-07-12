using Orange.Library.Values;

namespace Orange.Library
{
	public interface ITraitName
	{
		string MemberName
		{
			get;
		}

		Lambda Getter
		{
			get;
		}

		Lambda Setter
		{
			get;
		}
	}
}