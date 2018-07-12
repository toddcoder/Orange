using Orange.Library.Classes;
using Orange.Library.Values;

namespace Orange.Library.Parsers.Classes
{
	public interface IClassParser
	{
		ClassBuilder Builder
		{
			get;
			set;
		}

		bool EndOfClass
		{
			get;
		}

		Class.VisibilityType Visibility
		{
			get;
			set;
		}

		Class.ScopeType Scope
		{
			get;
			set;
		}
	}
}